using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockTowerCtrl : MonoBehaviour
{
    public Vector3 OriginalPos; // ���� ��ġ ��ǥ�� ����
    public Clock Clock_sc;
    public ParticleSystem[] Particle; //0:���� 1:����

    void Awake()
    {
        OriginalPos = this.transform.position; // ��ġ ��(Vector3)�� ����
        transitionTowerClock(true); // �ð�ž Ȱ��ȭ
        Particle[0].gameObject.SetActive(false); // ���� ��ƼŬ ��Ȱ��ȭ
        Particle[1].gameObject.SetActive(false); // ���� ��ƼŬ ��Ȱ��ȭ
    }
    public void ClockTowerSetting(int hour, int minute)
    {
        Clock_sc.hour = hour; // �ð�ž �ð� ��
        Clock_sc.minutes = minute; // �ð�ž �ð� ��
    }
    public void transitionTowerClock(bool activeB)
    {
        GameObject clockOb = Clock_sc.gameObject; // �ð� ������Ʈ
        // �θ� ������Ʈ�� Ȱ��/��Ȱ��
        //clockOb.SetActive(activeB);
        // ��Ȱ��ȭ�� �ڽı��� �����Ͽ� ó��
        foreach (Transform child in Clock_sc.gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (child != clockOb.transform)
                child.gameObject.SetActive(activeB);
        }
        // �ð� �ӵ� ����
        clockOb.GetComponent<Clock>().clockSpeed = activeB ? 1 : 0;
    }
}
