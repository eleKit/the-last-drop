namespace POLIMIGameCollective
{
    using UnityEngine;
    using System.Collections.Generic;

    static public class MethodExtensionForMonoBehaviourTransform
    {
        /// <summary>
        /// Gets or add a component. Usage example:
        /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
        /// </summary>
        static public T GetOrAddComponent<T>(this Component c) where T : Component
        {
            T result = c.GetComponent<T>();
            if (result == null)
                result = c.gameObject.AddComponent<T>();
            return result;
        }
	}

}