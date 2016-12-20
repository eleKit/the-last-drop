using UnityEngine;

namespace POLIMIGameCollective {

	[System.Serializable]
	public class MyComponent {
		public string anotherGlobalVar = "yeah";
	}

	public class ToolboxExample : MonoBehaviour {
		void Awake () {
			Debug.Log(Toolbox.Instance.myGlobalVar);

			Toolbox toolbox = Toolbox.Instance;
			Debug.Log(toolbox.language.current);

			// (optional) runtime registration of global objects
//			MyComponent myComponent = Toolbox.Instance.RegisterComponent<MyComponent>();
//			Debug.Log(myComponent.anotherGlobalVar);
//			Debug.Log(Toolbox.Instance.GetComponent<MyComponent>().anotherGlobalVar); // GetComponent is not recommended
//			Destroy(myComponent);
		}
	}

}