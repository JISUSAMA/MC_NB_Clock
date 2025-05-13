using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockTowerCtrl : MonoBehaviour
{
    public Vector3 OriginalPos; // 원래 위치 좌표만 저장
    public Clock Clock_sc;
    public ParticleSystem[] Particle; //0:실패 1:성공

    void Awake()
    {
        OriginalPos = this.transform.position; // 위치 값(Vector3)을 저장
        transitionTowerClock(true); // 시계탑 활성화
        Particle[0].gameObject.SetActive(false); // 실패 파티클 비활성화
        Particle[1].gameObject.SetActive(false); // 성공 파티클 비활성화
    }
    public void ClockTowerSetting(int hour, int minute)
    {
        Clock_sc.hour = hour; // 시계탑 시계 시
        Clock_sc.minutes = minute; // 시계탑 시계 분
    }
    public void transitionTowerClock(bool activeB)
    {
        GameObject clockOb = Clock_sc.gameObject; // 시계 오브젝트
        // 부모 오브젝트도 활성/비활성
        //clockOb.SetActive(activeB);
        // 비활성화된 자식까지 포함하여 처리
        foreach (Transform child in Clock_sc.gameObject.GetComponentsInChildren<Transform>(true))
        {
            if (child != clockOb.transform)
                child.gameObject.SetActive(activeB);
        }
        // 시계 속도 설정
        clockOb.GetComponent<Clock>().clockSpeed = activeB ? 1 : 0;
    }
}
