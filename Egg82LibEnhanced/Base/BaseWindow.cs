using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Graphics;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Startup;
using Egg82LibEnhanced.Utils;
using FarseerPhysics.Dynamics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Base {
	public class BaseWindow {
		//vars
		internal event EventHandler GainedFocus = null;
		internal event EventHandler<KeyEventArgs> KeyPressed = null;
		internal event EventHandler<KeyEventArgs> KeyReleased = null;
		internal event EventHandler<MouseMoveEventArgs> MouseMoved = null;
		internal event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled = null;
		internal event EventHandler<MouseButtonEventArgs> MouseButtonPressed = null;
		internal event EventHandler<MouseButtonEventArgs> MouseButtonReleased = null;
		
		private IGameEngine gameEngine = (IGameEngine) ServiceLocator.GetService(typeof(IGameEngine));
		private IPhysicsEngine physicsEngine = (IPhysicsEngine) ServiceLocator.GetService(typeof(IPhysicsEngine));
		private IInputEngine inputEngine = (IInputEngine) ServiceLocator.GetService(typeof(IInputEngine));

		private List<BaseState> states = new List<BaseState>();
		private World _physicsWorld = null;
		//private PhysicsWorld _physicsWorld = null;

		private QuadTree<DisplayObject> _quadTree = null;
		private PreciseRectangle oldSize = new PreciseRectangle();

		private RenderWindow window = null;
		private volatile bool _fullscreen = false;
		private Styles alternateStyles = Styles.Fullscreen;
		private Styles styles = Styles.None;
		private Texture _icon = null;
		private Color _color = new Color(255, 255, 255, 255);
		private string _title = null;
		private bool previousVsync = false;
		private volatile bool _vSync = false;
		private uint _antiAliasing = 0;
		private bool _cursorVisible = true;
		private bool _visible = true;

		//constructor
		public BaseWindow(double width, double height, string title, Styles style, bool vSync, uint antiAliasing) {
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

			window = new RenderWindow(new VideoMode((uint) width, (uint) height), title, style, new ContextSettings(24, 8, antiAliasing));

			styles = style;
			_fullscreen = ((style & Styles.Fullscreen) != Styles.None) ? true : false;
			_title = title;
			previousVsync = _vSync = vSync;
			_antiAliasing = antiAliasing;

			_icon = TextureUtil.FromBitmap(new System.Drawing.Bitmap(1, 1));
			window.SetIcon(_icon.Size.X, _icon.Size.Y, TextureUtil.ToBytes(_icon));

			window.Closed += onClosed;
			window.Resized += onResize;

			window.GainedFocus += onFocused;
			window.KeyPressed += onKeyDown;
			window.KeyReleased += onKeyUp;
			window.MouseMoved += onMouseMove;
			window.MouseWheelScrolled += onMouseWheel;
			window.MouseButtonPressed += onMouseDown;
			window.MouseButtonReleased += onMouseUp;

			window.SetVerticalSyncEnabled(vSync);
			window.SetView(new View(new FloatRect(0.0f, 0.0f, (float) width, (float) height)));
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle(0.0d, 0.0d, width, height));

			window.SetActive(false);

			_physicsWorld = physicsEngine.CreateWorld();
			inputEngine.AddWindow(this);
			gameEngine.AddWindow(this);
			Start.AddWindow(this);
		}
		internal BaseWindow(World physicsWorld, List<BaseState> states, Styles alternateStyle, Texture icon, double width, double height, string title, Styles style, bool vSync, uint antiAliasing) {
			window = new RenderWindow(new VideoMode((uint) width, (uint) height), title, style, new ContextSettings(24, 8, antiAliasing));

			alternateStyles = alternateStyle;
			styles = style;
			_fullscreen = ((style & Styles.Fullscreen) != Styles.None) ? true : false;
			_title = title;
			previousVsync = _vSync = vSync;
			_antiAliasing = antiAliasing;

			_icon = icon;
			window.SetIcon(_icon.Size.X, _icon.Size.Y, TextureUtil.ToBytes(_icon));

			window.Closed += onClosed;
			window.Resized += onResize;

			window.GainedFocus += onFocused;
			window.KeyPressed += onKeyDown;
			window.KeyReleased += onKeyUp;
			window.MouseMoved += onMouseMove;
			window.MouseWheelScrolled += onMouseWheel;
			window.MouseButtonPressed += onMouseDown;
			window.MouseButtonReleased += onMouseUp;

			window.SetVerticalSyncEnabled(vSync);
			window.SetView(new View(new FloatRect(0.0f, 0.0f, (float) width, (float) height)));
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle(0.0d, 0.0d, width, height));
			
			for (int i = 0; i < states.Count; i++) {
				this.states.Add(states[i]);
				states[i].Window = this;
			}

			window.SetActive(false);

			_physicsWorld = physicsWorld;
			inputEngine.AddWindow(this);
			gameEngine.AddWindow(this);
		}
		~BaseWindow() {
			window.Closed -= onClosed;
			window.Resized -= onResize;

			window.GainedFocus -= onFocused;
			window.KeyPressed -= onKeyDown;
			window.KeyReleased -= onKeyUp;
			window.MouseMoved -= onMouseMove;
			window.MouseWheelScrolled -= onMouseWheel;
			window.MouseButtonPressed -= onMouseDown;
			window.MouseButtonReleased -= onMouseUp;

			gameEngine.RemoveWindow(this);
			inputEngine.RemoveWindow(this);
			Start.RemoveWindow(this);
		}

		//public
		public bool Fullscreen {
			get {
				return _fullscreen;
			}
			set {
				_fullscreen = value;
			}
		}
		public string Title {
			get {
				return _title;
			}
			set {
				if (value == null) {
					value = "";
				}
				if (value == _title) {
					return;
				}

				_title = value;
				window.SetTitle(value);
			}
		}
		public uint AntiAliasing {
			get {
				return _antiAliasing;
			}
		}
		public bool CursorVisible {
			get {
				return _cursorVisible;
			}
			set {
				if (value == _cursorVisible) {
					return;
				}

				_cursorVisible = value;
				window.SetMouseCursorVisible(value);
			}
		}
		public bool Visible {
			get {
				return _visible;
			}
			set {
				if (value == _visible) {
					return;
				}

				_visible = value;
				window.SetVisible(value);
			}
		}
		public bool VerticalSync {
			get {
				return _vSync;
			}
			set {
				if (value == _vSync) {
					return;
				}

				_vSync = value;
			}
		}
		public Texture Icon {
			get {
				return _icon;
			}
			set {
				if (value == null || value == _icon) {
					return;
				}
				
				_icon = value;
				window.SetIcon(value.Size.X, value.Size.Y, TextureUtil.ToBytes(value));
			}
		}

		public double X {
			get {
				return (double) window.Position.X;
			}
			set {
				if (value == window.Position.X || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				window.Position = new Vector2i((int) value, window.Position.Y);
			}
		}
		public double Y {
			get {
				return (double) window.Position.Y;
			}
			set {
				if (value == window.Position.Y || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}

				window.Position = new Vector2i(window.Position.X, (int) value);
			}
		}
		public double Width {
			get {
				return (double) window.Size.X;
			}
			set {
				window.Size = new Vector2u((uint) value, window.Size.Y);
			}
		}
		public double Height {
			get {
				return (double) window.Size.Y;
			}
			set {
				window.Size = new Vector2u(window.Size.X, (uint) value);
			}
		}

		public void Update(double deltaTime) {
			for (int i = 0; i < states.Count; i++) {
				if (states[i].Ready) {
					states[i].Update(deltaTime);
				}
			}
		}
		public void SwapBuffers() {
			for (int i = 0; i < states.Count; i++) {
				if (states[i].Ready) {
					states[i].SwapBuffers();
				}
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

			state.Window = this;
			states.Insert(index, state);
			state.Enter();
		}
		public void RemoveState(BaseState state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			int index = states.IndexOf(state);
			if (index == -1) {
				return;
			}
			
			state.Exit();
			states.RemoveAt(index);
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
			if (index == -1) {
				return false;
			}

			oldState.Ready = false;
			oldState.Exit();
			oldState.Window = null;
			newState.Window = this;
			states[index] = newState;
			newState.Enter();
			newState.Ready = true;
			return true;
		}

		public bool IsOpen {
			get {
				return window.IsOpen;
			}
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

		public void Close() {
			window.Close();
		}

		//private
		internal void Draw() {
			if (_vSync != previousVsync) {
				window.SetVerticalSyncEnabled(_vSync);
				previousVsync = _vSync;
			}
			
			//SetActive(true);
			window.Clear(Color.Transparent);
			for (int i = states.Count - 1; i >= 0; i--) {
				if (states[i].Ready) {
					states[i].Draw(window, Transform.Identity, _color);
				}
			}
			window.Display();
			//SetActive(false);
		}
		
		internal bool NeedsRepacement {
			get {
				if (_fullscreen && (styles & Styles.Fullscreen) == Styles.None) {
					return true;
				} else if (!_fullscreen && (styles & Styles.Fullscreen) != Styles.None) {
					return true;
				}
				return false;
			}
		}
		internal BaseWindow GetReplacement() {
			gameEngine.RemoveWindow(this);
			inputEngine.RemoveWindow(this);

			return new BaseWindow(_physicsWorld, states, styles, _icon, (double) window.Size.X, (double) window.Size.Y, _title, alternateStyles, _vSync, _antiAliasing);
		}
		internal void DispatchEvents() {
			window.DispatchEvents();
		}

		private void onKeyDown(object sender, KeyEventArgs e) {
			KeyPressed?.Invoke(this, e);
		}
		private void onKeyUp(object sender, KeyEventArgs e) {
			KeyReleased?.Invoke(this, e);
		}

		private void onMouseMove(object sender, MouseMoveEventArgs e) {
			MouseMoved?.Invoke(this, e);
		}
		private void onMouseWheel(object sender, MouseWheelScrollEventArgs e) {
			MouseWheelScrolled?.Invoke(this, e);
		}
		private void onMouseDown(object sender, MouseButtonEventArgs e) {
			MouseButtonPressed?.Invoke(this, e);
		}
		private void onMouseUp(object sender, MouseButtonEventArgs e) {
			MouseButtonReleased?.Invoke(this, e);
		}

		private void onFocused(object sender, EventArgs e) {
			GainedFocus?.Invoke(this, e);
		}

		private void onClosed(object sender, EventArgs e) {
			window.Close();
		}
		private void onResize(object sender, SizeEventArgs e) {
			List<DisplayObject> objects = _quadTree.GetAllObjects();
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle((double) e.Width, (double) e.Height));
			window.SetView(new View(new FloatRect(0.0f, 0.0f, (float) e.Width, (float) e.Height)));
			
			for (int i = 0; i < objects.Count; i++) {
				_quadTree.Add(objects[i]);
			}
			for (int i = 0; i < states.Count; i++) {
				states[i].Resize((double) e.Width, (double) e.Height);
			}
		}
	}
}
