using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Geom;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Startup;
using Egg82LibEnhanced.Utils;
using FarseerPhysics.Dynamics;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Display {
	public class Window {
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

		private List<State> states = new List<State>();
		private World _physicsWorld = null;
		//private PhysicsWorld _physicsWorld = null;

		private QuadTree<DisplayObject> _quadTree = null;
		private PreciseRectangle oldSize = new PreciseRectangle();

		private RenderWindow window = null;
		private PreciseRectangle viewport = new PreciseRectangle();
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
		/// <summary>
		/// A physical window that displays on the screen. This is where all DisplayObjects will live when drawn to the screen. You may have multiple windows.
		/// </summary>
		/// <param name="width">The window's width.</param>
		/// <param name="height">The window's height.</param>
		/// <param name="title">The window's title.</param>
		/// <param name="style">Any style flags to apply to the window.</param>
		/// <param name="vSync">(optional) Whether or not this window syncs its rendering to the refresh rate of the monitor.</param>
		/// <param name="antiAliasing">(optional) The amount of AntiAliasing to use. More = slower but lexx pixelated.</param>
		public Window(double width, double height, string title, Styles style, bool vSync = true, uint antiAliasing = 16) {
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
			IntRect vp = window.GetViewport(window.GetView());
			viewport.Width = (double) vp.Width;
			viewport.Height = (double) vp.Height;
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle(0.0d, 0.0d, viewport.Width, viewport.Height));

			window.SetActive(false);

			_physicsWorld = physicsEngine.CreateWorld();
			inputEngine.AddWindow(this);
			gameEngine.AddWindow(this);
			Start.AddWindow(this);
		}
		internal Window(World physicsWorld, List<State> states, Styles alternateStyle, Texture icon, double width, double height, string title, Styles style, bool vSync, uint antiAliasing) {
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
			IntRect vp = window.GetViewport(window.GetView());
			viewport.Width = (double) vp.Width;
			viewport.Height = (double) vp.Height;
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle(0.0d, 0.0d, viewport.Width, viewport.Height));
			
			for (int i = 0; i < states.Count; i++) {
				this.states.Add(states[i]);
				states[i].SetWindow(this);
			}

			window.SetActive(false);

			_physicsWorld = physicsWorld;
			inputEngine.AddWindow(this);
			gameEngine.AddWindow(this);
		}
		~Window() {
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
		/// <summary>
		/// Whether or not this window is in fullscreen mode. Setting this value creates a new window.
		/// </summary>
		public bool Fullscreen {
			get {
				return _fullscreen;
			}
			set {
				_fullscreen = value;
			}
		}
		/// <summary>
		/// This window's title.
		/// </summary>
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
		/// <summary>
		/// The amount of AntiAliasing in this window. Setting this value creates a new window.
		/// </summary>
		public uint AntiAliasing {
			get {
				return _antiAliasing;
			}
			set {
				_antiAliasing = value;
			}
		}
		/// <summary>
		/// Whether or not the mouse cursor is visible while inside this window.
		/// </summary>
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
		/// <summary>
		/// Whether or not this window is visible.
		/// </summary>
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
		/// <summary>
		/// Whether or not this window syncs its rendering to the refresh rate of the monitor.
		/// </summary>
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
		/// <summary>
		/// The icon for this window.
		/// </summary>
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
		/// <summary>
		/// Whether or not the window is open.
		/// </summary>
		public bool IsOpen {
			get {
				return window.IsOpen;
			}
		}

		/// <summary>
		/// This window's X position, relative to the main (spawning) monitor and including any border it has.
		/// </summary>
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
		/// <summary>
		/// This window's Y position, relative to the main (spawning) monitor and including any border it has.
		/// </summary>
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
		/// <summary>
		/// This window's internal width. This excludes any border it has.
		/// </summary>
		public double Width {
			get {
				return viewport.Width;
			}
			set {
				value += window.Size.X - viewport.Width;
				window.Size = new Vector2u((uint) value, window.Size.Y);
			}
		}
		/// <summary>
		/// This window's internal height. This excludes any border it has.
		/// </summary>
		public double Height {
			get {
				return viewport.Height;
			}
			set {
				value += window.Size.Y - viewport.Height;
				window.Size = new Vector2u(window.Size.X, (uint) value);
			}
		}

		/// <summary>
		/// Adds a child BaseState to the BaseWindow.
		/// </summary>
		/// <param name="state">The child BaseState to add.</param>
		/// <param name="index">(optional) The index at which to add the child. The default is the top of the stack.</param>
		public void AddState(State state, int index = 0) {
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

			state.SetWindow(this);
			states.Insert(index, state);
			state.Enter();
		}
		/// <summary>
		/// Removes a child BaseState from the BaseWindow.
		/// </summary>
		/// <param name="state">The child BaseState to remove.</param>
		public void RemoveState(State state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}
			int index = states.IndexOf(state);
			if (index == -1) {
				return;
			}
			
			state.Exit();
			states.RemoveAt(index);
			state.SetWindow(null);
		}
		/// <summary>
		/// Returns a child BaseState at the specified index.
		/// </summary>
		/// <param name="index">The index of the child.</param>
		/// <returns>The child, or null if the speicified index is out-of-bounds.</returns>
		public State GetStateAt(int index) {
			if (index < 0 || index >= states.Count) {
				return null;
			}

			return states[index];
		}
		/// <summary>
		/// Returns the index of the provided child BaseState.
		/// </summary>
		/// <param name="state">The child.</param>
		/// <returns>The index of the provided child, or -1 if the provided BaseState is not a child of this BaseWindow.</returns>
		public int IndexOf(State state) {
			if (state == null) {
				throw new ArgumentNullException("state");
			}

			return states.IndexOf(state);
		}
		/// <summary>
		/// Sets the index of the provided child BaseState to the provided index.
		/// </summary>
		/// <param name="state">The child.</param>
		/// <param name="index">The index to set the child to.</param>
		public void SetIndex(State state, int index) {
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
			if (currentIndex == -1 || index == currentIndex) {
				return;
			}

			states.RemoveAt(currentIndex);
			states.Insert(index, state);
		}

		/// <summary>
		/// Tries to swap states from the old BaseState provided to the provided new BaseState.
		/// This swaps at the old BaseState's current index as opposed to adding a new state to the stack.
		/// </summary>
		/// <param name="oldState">The old state from which to swap.</param>
		/// <param name="newState">The new state to swap to.</param>
		/// <returns>True if successful, false if unsuccessful.</returns>
		public bool TrySwapStates(State oldState, State newState) {
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
			oldState.SetWindow(null);
			newState.SetWindow(this);
			states[index] = newState;
			newState.Enter();
			newState.Ready = true;
			return true;
		}

		/// <summary>
		/// The number of child states this BaseWindow has.
		/// </summary>
		public int NumStates {
			get {
				return states.Count;
			}
		}

		/// <summary>
		/// The physics world attached to this window. Returns null if not using the physics engine.
		/// </summary>
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

		/// <summary>
		/// The quad tree attached to this window. Contains all DisplayObjects this window has, visible or not.
		/// The quad tree uses each DisplayObject's GlobalBounds property as the stored bounds.
		/// </summary>
		public QuadTree<DisplayObject> QuadTree {
			get {
				return _quadTree;
			}
		}

		/// <summary>
		/// Closes the window.
		/// </summary>
		public void Close() {
			window.Close();
		}

		//private
		internal void Update(double deltaTime) {
			for (int i = 0; i < states.Count; i++) {
				if (states[i].Ready) {
					states[i].Update(deltaTime);
				}
			}
		}
		internal void SwapBuffers() {
			for (int i = 0; i < states.Count; i++) {
				if (states[i].Ready) {
					states[i].SwapBuffers();
				}
			}
		}
		internal void Draw() {
			if (_vSync != previousVsync) {
				window.SetVerticalSyncEnabled(_vSync);
				previousVsync = _vSync;
			}
			
			window.Clear(Color.Transparent);
			for (int i = states.Count - 1; i >= 0; i--) {
				if (states[i].Ready) {
					states[i].Draw(window, Transform.Identity, _color);
				}
			}
			window.Display();
		}
		
		internal bool NeedsRepacement {
			get {
				if (_fullscreen && (styles & Styles.Fullscreen) == Styles.None) {
					return true;
				} else if (!_fullscreen && (styles & Styles.Fullscreen) != Styles.None) {
					return true;
				}
				if (_antiAliasing != window.Settings.AntialiasingLevel) {
					return true;
				}
				return false;
			}
		}
		internal Window GetReplacement() {
			gameEngine.RemoveWindow(this);
			inputEngine.RemoveWindow(this);

			return new Window(_physicsWorld, states, styles, _icon, (double) window.Size.X, (double) window.Size.Y, _title, alternateStyles, _vSync, _antiAliasing);
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
			window.SetView(new View(new FloatRect(0.0f, 0.0f, (float) e.Width, (float) e.Height)));
			IntRect vp = window.GetViewport(window.GetView());
			viewport.Width = (double) vp.Width;
			viewport.Height = (double) vp.Height;
			_quadTree = new QuadTree<DisplayObject>(new PreciseRectangle(viewport.Width, viewport.Height));

			for (int i = 0; i < objects.Count; i++) {
				_quadTree.Add(objects[i]);
			}
			for (int i = 0; i < states.Count; i++) {
				states[i].Resize((double) e.Width, (double) e.Height);
			}
		}
	}
}
