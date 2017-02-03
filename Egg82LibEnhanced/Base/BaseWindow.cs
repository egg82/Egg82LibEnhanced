using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Graphics;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Patterns.Interfaces;
using Egg82LibEnhanced.Startup;
using Egg82LibEnhanced.Utils;
using FarseerPhysics.Dynamics;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Base {
	public class BaseWindow : RenderWindow, IUpdatable {
		//vars
		private IGameEngine gameEngine = (IGameEngine) ServiceLocator.GetService(typeof(IGameEngine));
		private IPhysicsEngine physicsEngine = (IPhysicsEngine) ServiceLocator.GetService(typeof(IPhysicsEngine));
		private IInputEngine inputEngine = (IInputEngine) ServiceLocator.GetService(typeof(IInputEngine));

		private List<BaseState> states = new List<BaseState>();
		private World _physicsWorld = null;
		//private PhysicsWorld _physicsWorld = null;

		private QuadTree<DisplayObject> _quadTree = null;
		private PreciseRectangle oldSize = new PreciseRectangle();
		
		private bool _isFinalized = false;

		//constructor
		public BaseWindow(double width, double height, string title, Styles style, bool vSync, uint antialiasing) : base(new VideoMode((uint) width, (uint) height), title, style, new ContextSettings(0, 0, antialiasing)){
			if (double.IsNaN(width)) {
				throw new ArgumentNullException("width");
			}
			if (double.IsInfinity(width) || width < 1.0d) {
				throw new InvalidOperationException("width cannot be less than 1 or infinity.");
			}
			if (double.IsNaN(height)) {
				throw new ArgumentNullException("height");
			}
			if (double.IsInfinity(height) || height < 1.0d) {
				throw new InvalidOperationException("height cannot be less than 1 or infinity.");
			}

			Closed += onClosed;
			Resized += onResize;

			SetVerticalSyncEnabled(vSync);
			SetView(new View(new FloatRect(0.0f, 0.0f, (float) width, (float) height)));
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle(0.0d, 0.0d, Width, Height));

			SetActive(false);

			_physicsWorld = physicsEngine.CreateWorld();
			inputEngine.AddWindow(this);
			gameEngine.AddWindow(this);
			Start.addWindow(this);
		}
		~BaseWindow() {
			_isFinalized = true;
			gameEngine.RemoveWindow(this);
			inputEngine.RemoveWindow(this);
			physicsEngine.RemoveWorld(_physicsWorld);
			Start.removeWindow(this);
		}

		//public
		public bool Valid {
			get {
				return _isFinalized;
			}
		}

		public void UpdateEvents() {
			/*Event e;
			while (PollEvent(out e)) {
				if (e.Type == EventType.Closed) {
					while (states.Count > 0) {
						BaseState state = states[0];
						states.RemoveAt(0);
						state.OnExit();
						state.Window = null;
					}
					gameEngine.RemoveWindow(this);
					inputEngine.RemoveWindow(this);
					physicsEngine.RemoveWorld(_physicsWorld);
					Close();
				} else if (e.Type == EventType.Resized) {
					onResize();
				}
			}*/
			DispatchEvents();
		}

		public double X {
			get {
				return (double) Position.X;
			}
		}
		public double Y {
			get {
				return (double) Position.Y;
			}
		}
		public double Width {
			get {
				return (double) Size.X;
			}
		}
		public double Height {
			get {
				return (double) Size.Y;
			}
		}

		public void Update(double deltaTime) {
			for (int i = 0; i < states.Count; i++) {
				states[i].Update(deltaTime);
			}
		}
		public void SwapBuffers() {
			for (int i = 0; i < states.Count; i++) {
				states[i].SwapBuffers();
			}
		}
		
		public void AddState(BaseState state, int index = 0) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			if (states.Contains(state)) {
				return;
			}
			if (index > states.Count) {
				index = states.Count;
			}
			if (index < 0) {
				index = 0;
			}

			states.Insert(index, state);
			state.Window = this;
			state.OnEnter();
		}
		public void RemoveState(BaseState state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			int index = states.IndexOf(state);
			if (index == -1) {
				return;
			}

			states.RemoveAt(index);
			state.OnExit();
			state.Window = null;
		}
		public BaseState GetStateAt(int index) {
			if (index < 0 || index >= states.Count) {
				return null;
			}
			return states[index];
		}
		public int IndexOf(BaseState state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}

			return states.IndexOf(state);
		}
		public void SetIndex(BaseState state, int index) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			if (index > states.Count) {
				index = states.Count;
			}
			if (index < 0) {
				index = 0;
			}

			int currentIndex = states.IndexOf(state);
			if (currentIndex == -1) {
				return;
			}

			states.RemoveAt(currentIndex);
			states.Insert(index, state);
		}

		public bool TrySwapStates(BaseState oldState, BaseState newState) {
			if (oldState == null) {
				throw new ArgumentNullException("oldState");
			}
			if (newState == null) {
				throw new ArgumentNullException("newState");
			}

			int index = states.IndexOf(oldState);
			if (index == -1 || !oldState.HasExitState(newState.GetType())) {
				return false;
			}

			oldState.OnExit();
			oldState.Window = null;
			states[index] = newState;
			newState.Window = this;
			newState.OnEnter();
			return true;
		}

		public int NumStates {
			get {
				return states.Count;
			}
		}

		public World PhysicsWorld {
			get {
				return _physicsWorld;
			}
		}
		/*public PhysicsWorld PhysicsWorld {
			get {
				return _physicsWorld;
			}
		}*/

		public QuadTree<DisplayObject> QuadTree {
			get {
				return _quadTree;
			}
		}

		//private
		internal void Draw() {
			SetActive(true);
			Clear(Color.Transparent);
			for (int i = states.Count - 1; i >= 0; i--) {
				states[i].Draw(this, Transform.Identity);
			}
			Display();
			SetActive(false);
		}

		private void onClosed(object sender, EventArgs e) {
			Close();
		}
		private void onResize(object sender, SizeEventArgs e) {
			List<DisplayObject> objects = _quadTree.GetAllObjects();
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle((double) e.Width, (double) e.Height));
			SetView(new View(new FloatRect(0.0f, 0.0f, (float) Width, (float) Height)));
			
			for (int i = 0; i < objects.Count; i++) {
				_quadTree.Add(objects[i]);
			}
			for (int i = 0; i < states.Count; i++) {
				states[i].OnResize(Width, Height);
			}
		}
	}
}
