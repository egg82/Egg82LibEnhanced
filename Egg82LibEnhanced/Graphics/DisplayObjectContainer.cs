using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Graphics {
	public abstract class DisplayObjectContainer : DisplayObject {
		//vars
		private List<DisplayObject> children = new List<DisplayObject>();

		//constructor
		public DisplayObjectContainer() {

		}

		//public
		public new void Update(double deltaTime) {
			for (int i = 0; i < children.Count; i++) {
				children[i].Update(deltaTime);
			}
			base.Update(deltaTime);
		}
		public new void SwapBuffers() {
			for (int i = 0; i < children.Count; i++) {
				children[i].SwapBuffers();
			}
			base.SwapBuffers();
		}
		public new void Draw(RenderTarget target, Transform parentTransform) {
			base.Draw(target, parentTransform);

			for (int i = children.Count - 1; i >= 0; i--) {
				children[i].Draw(target, GlobalTransform);
			}
		}

		public void AddChild(DisplayObject obj, int index = 0) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}
			if (obj.Parent != null) {
				throw new Exception("object cannot be added if it already has a parent.");
			}

			if (children.Contains(obj)) {
				return;
			}
			if (index > children.Count) {
				index = children.Count;
			}
			if (index < 0) {
				index = 0;
			}

			children.Insert(index, obj);
			obj.Parent = this;
			obj.Window = Window;
		}
		public void RemoveChild(DisplayObject obj) {
			if (obj == null) {
				throw new ArgumentNullException("obj");
			}

			int index = children.IndexOf(obj);
			if (index == -1) {
				throw new Exception("object is not a child of this object.");
			}

			children.RemoveAt(index);
			obj.Parent = null;
			obj.Window = null;
		}
		public DisplayObject GetChildAt(int index) {
			if (index < 0 || index >= children.Count) {
				throw new IndexOutOfRangeException();
			}
			return children[index];
		}
		public int GetChildIndex(DisplayObject child) {
			if (child == null) {
				throw new ArgumentNullException("child");
			}

			return children.IndexOf(child);
		}
		public void SetChildIndex(DisplayObject child, int index) {
			if (child == null) {
				throw new ArgumentNullException("child");
			}
			if (index > children.Count) {
				index = children.Count;
			}
			if (index < 0) {
				index = 0;
			}

			int currentIndex = children.IndexOf(child);
			if (currentIndex == -1) {
				return;
			}

			children.RemoveAt(currentIndex);
			children.Insert(index, child);
		}

		//private

	}
}
