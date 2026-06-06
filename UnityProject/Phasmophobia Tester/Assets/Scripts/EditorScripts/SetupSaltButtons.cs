using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Serialization;
using UnityEngine.Events;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class SetupSaltButtons : MonoBehaviour
{
    [SerializeField] private string buttonName, textName;
    [SerializeField] private GameManager gameManager;

    [Button("Setup Buttons")]
    private void SetupText()
    {
#if UNITY_EDITOR
        int count = 0;
        foreach (Transform t in transform)
        {
            t.TryGetComponent<Button>(out Button btn);
            if (btn == null) return;

            btn.onClick = new Button.ButtonClickedEvent();
            UnityEventTools.AddObjectPersistentListener<Transform>(btn.onClick, new UnityAction<Transform>(gameManager.PlaceSalt), t);
            EditorUtility.SetDirty(btn);

            t.name = buttonName + " " + count.ToString();
            t.GetChild(0).name = textName + " " + count.ToString();
            t.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();

            count++;
        }
        if (gameObject.scene.IsValid())
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }
}
