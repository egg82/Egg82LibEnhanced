using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Display.Interactable;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.Drawing;
using System.Xml;
using Test.Registries;
using Test.States.Door;

namespace Test.States {
	class GameDoorInitState : State {
		//vars
		private TextureAtlas doorsAtlas = null;
		private TextureAtlas foodAtlas = null;
		private XmlDocument foodAtlasXml = new XmlDocument();

		private IRegistry<string> gameRegistry = null;
		
		private TextBox loadingText = null;
		private int loaded = 0;

		//constructor
		public GameDoorInitState() {

		}

		//public

		//private
		protected override void OnEnter() {
			ServiceLocator.ProvideService(typeof(GameDoorRegistry));

			loadingText = new TextBox(new Font(FontUtil.GetFont("pixel"), 18.0f, FontStyle.Regular), "Load'n");
			loadingText.X = Window.Width / 2.0d - loadingText.Width / 2.0d;
			loadingText.Y = Window.Height / 2.0d - loadingText.Height / 2.0d;
			AddChild(loadingText);
			
		}
		protected override void OnExit() {
			
		}
		protected override void OnUpdate(double deltaTime) {
			if (loaded == 1) {
				string doorsAtlasPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Images" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Doors" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "doors.png";
				doorsAtlas = new TextureAtlas(TextureUtil.BitmapFromBytes(FileUtil.Read(doorsAtlasPath, 0)), 64, 24);
			} else if (loaded == 2) {
				string foodAtlasPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Images" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Doors" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "food.png";
				string foodAtlasXmlPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Images" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Doors" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "food.xml";
				foodAtlasXml.Load(foodAtlasXmlPath);
				foodAtlas = new TextureAtlas(TextureUtil.BitmapFromBytes(FileUtil.Read(foodAtlasPath, 0)), foodAtlasXml);
			} else if (loaded == 3) {
				string mainFontPath = FileUtil.CURRENT_DIRECTORY + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + ".." + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Assets" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Fonts" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "Doors" + FileUtil.DIRECTORY_SEPARATOR_CHAR + "neuropol x rg.ttf";
				FontUtil.AddFont("main", FileUtil.Read(mainFontPath, 0));
			} else if (loaded > 3) {
				gameRegistry = ServiceLocator.GetService<GameDoorRegistry>();
				gameRegistry.SetRegister("doors", doorsAtlas);
				gameRegistry.SetRegister("food", foodAtlas);

				loadingText.Text = "Done!";
				loadingText.X = Window.Width / 2.0d - loadingText.Width / 2.0d;
				loadingText.Y = Window.Height / 2.0d - loadingText.Height / 2.0d;

				//Window.SwapStates(this, new IntroState());
				Window.SwapStates(this, new GameState());
			}

			loaded++;
		}
	}
}
