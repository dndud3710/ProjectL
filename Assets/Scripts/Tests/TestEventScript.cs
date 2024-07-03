using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TestEventScript : MonoBehaviour
{
    public static TestEventScript Instance { get; private set; }

    [SerializeField] Button ChangeWeaponButton;
    [SerializeField] Button AttackButton;
    [SerializeField] Button CheckButton;
    [SerializeField] Button CounterAttackButton;
    [SerializeField] Button TeleportButton;
    [SerializeField] InputField AttackComboInput;

    DeathKnight Owner;

    public UnityEvent ChangeWeaponEvent;
    public UnityEvent AttackEvent;
    public UnityEvent CheckEvent;
    public UnityEvent CounterEvent;
    public UnityEvent TeleportEvent;


    private void Awake()
    {
        Instance = this;

        ChangeWeaponButton.onClick.AddListener(() => ChangeWeaponEvent?.Invoke());
        AttackButton.onClick.AddListener(() => AttackEvent?.Invoke());
        CheckButton.onClick.AddListener(() => CheckEvent?.Invoke());
        CounterAttackButton.onClick.AddListener(() => CounterEvent?.Invoke());
        TeleportButton.onClick.AddListener(() => TeleportEvent?.Invoke());
        AttackComboInput.onValueChanged.AddListener((x) =>
        {
            int result;

            if (int.TryParse(AttackComboInput.text, out result) == true)
            {
                // Owner.photonView.RPC("ChangeAnimRPC", RpcTarget.All, "AttackCombo", result);
                Owner.ChangeAnimRPC("AttackCombo", result);
                // Owner.ChangeAnim("AttackCombo", result);
            }
        });

    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);

        Owner = GameObject.Find("Boss(Clone)")?.GetComponent<DeathKnight>();

        yield return null;

        if(Owner == null)
        {
            Owner = GameObject.Find("Boss").GetComponent<DeathKnight>();
        }

    }
}
