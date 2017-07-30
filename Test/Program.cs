using Egg82LibEnhanced.Startup;
using System;
using Test.States;

namespace Test {
	class Program {
		static void Main(string[] args) {
			Init init = new InitState();
			init.Begin();
			Environment.Exit(0);
		}
	}
}
