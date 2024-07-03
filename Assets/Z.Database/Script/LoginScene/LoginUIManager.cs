using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIManager : MonoBehaviour
{
    public static LoginUIManager instance;

    Transform Canvas;
    GameObject LogInButton;
    GameObject CharacterSelectPanel_Prefab;

    public CharacterSelect characterselect {  get; private set; }
    
    private void Awake()
    {
        instance = this;
        CharacterSelectPanel_Prefab = Resources.Load<GameObject>("UI/CharacterSelect");
        LogInButton = Resources.Load<GameObject>("LoginPanel");
    }
    private void Start()
    {
        Canvas = GameObject.Find("Canvas").transform;

        var f = Instantiate(CharacterSelectPanel_Prefab, Canvas);
        characterselect = f.GetComponent<CharacterSelect>();
        characterselect.SelectAction += SelectButtonCreate;
}
    private void SelectButtonCreate()
    {
        var g = Instantiate(LogInButton.gameObject, Canvas);
        g.transform.Find("LogInButton").GetComponent<Button>().onClick.AddListener(ButtonClick);

    }
    private void ButtonClick()
    {
        if (!characterselect.ErrorBool()) characterselect.OnErrorPanel();
        else LoginPhotonManager.Instance.LogInButton();
    }
}
