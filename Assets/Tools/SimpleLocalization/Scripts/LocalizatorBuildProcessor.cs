#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class LocalizatorBuildProcessor : IPreprocessBuild
{
    public int callbackOrder => throw new System.NotImplementedException();

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        throw new System.NotImplementedException();
    }
}

#endif
