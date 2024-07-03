using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HjSceneManager : MonoBehaviourPun
{
    [SerializeField] Button MobTakeDamageButton;
    [SerializeField] Button FirstPatternButton;
    [SerializeField] Button AnimStartButton;

    [SerializeField] int enemyCount;

    TestMob[] testMobs;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        if (PhotonNetwork.IsMasterClient == true)
        {
            GameObject player = GameObject.Find("TestPlayer2");
            testMobs = GameManager.Instance.factory.GetMonster(enemyCount);

            for (int i = 0; i < enemyCount; i++)
            {
                // �� ��ü�� ���� ������ ��� (�� ����)
                float angleInDegrees = 360f * i / enemyCount;
                float angleInRadians = angleInDegrees * Mathf.Deg2Rad; // �������� ��ȯ

                Vector3 spawnPosition = player.transform.position + new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians)) * 15;

                GameObject obj = PhotonNetwork.InstantiateRoomObject("Prefabs/Enemy/Mob/TestRagDollMob2", spawnPosition, Quaternion.Euler(0, -angleInDegrees, 0), 0, null);
                TestMob target = obj.GetComponent<TestMob>();
                testMobs[i] = target;

                MobTakeDamageButton.onClick.AddListener(() => target.TakeDamageRPC(30, target.CurTarget.position, 0));
            }
            GameObject boss = PhotonNetwork.InstantiateRoomObject("Prefabs/Enemy/Boss", Vector3.zero, Quaternion.identity, 0, null);

            FirstPatternButton.onClick.AddListener(() => PhotonNetwork.Instantiate("Prefabs/Enemy/Pattern/FirstPattern", Vector3.zero, Quaternion.identity));
            AnimStartButton.onClick.AddListener(() => boss.GetComponent<DeathKnight>().ChangeAnimRPC("Start"));
        }
        
    }

    private void Update()
    {
        /*
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mouseScreenPosition = Input.mousePosition;

            // ī�޶� ����Ͽ� ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
            Camera camera = Camera.main; // ���� ī�޶� ������
            Ray ray = camera.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;

            // Raycast�� ����Ͽ� ���� Ŭ���� ��ġ�� ������Ʈ ����
            if (Physics.Raycast(ray, out hit))
            {
                foreach (var mob in testMobs)
                {
                    mob.TakeDamageRPC(30, hit.point);
                }
            }
        }
        */
    }

}
