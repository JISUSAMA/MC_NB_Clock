# 🕰 지금 몇 시일까요?

> **시계 읽기 능력 향상**을 위한
> 초등학교 1학년 대상 **3D 인터랙티브 시간 학습 콘텐츠**

---

## 📌 개요

\*\*“지금 몇 시일까요?”\*\*는 아이들이 **아날로그 시계 읽기**, **하루 일과 시간 인식**,
**시침/분침의 원리**를 직관적이고 재미있게 익힐 수 있도록 구성된 **퍼즐형 시간 학습 콘텐츠**입니다.

---

## 🛠 개발 정보

| 항목       | 내용                        |
| -------- | ------------------------- |
| 엔진       | Unity 2022.3 LTS          |
| 지원 디바이스  | Leia Lume Pad 2, Android  |
| 주요 사용 기술 | DOTween, TextMeshPro, URP |
| 주요 UI 요소 | 3D 시계탑, 드래그 앤 드롭, 나레이션 패널 |

---

## 📚 학습 콘텐츠 구성 방식

### 🎯 Mission01: 시계 읽기 훈련

* **퀴즈 방식**: 시계탑의 시계에서 바늘을 드래그하여 정답을 맞춤
* **정답 시각 구성**: 시는 1\~12시, 분은 0, 15, 30, 45 중 랜덤
* **선택지**: 정답 + 랜덤 보기 2개
* **UI 요소**: `"몇 시 몇 분을 찾아주세요!"` (`QuestionText`) 안내 표시

---

### 🧩 Mission02: 하루 일과 시간 매칭

* **퀴즈 방식**: 주어진 장면에 어울리는 디지털 시계를 터치 선택
* **정답 시각 구성**: 미리 정의된 6개 시간 (예: 07:15, 08:30 등)
* **선택지**: 정답 + 3개 보기 (중복 방지 처리)
* **UI 요소**: 퀴즈 이미지(`QuizImage`) + 텍스트(`QuizTimeText`)

---

## 🖼️ 예시 이미지

### Mission01 - 시계 바늘 맞추기

| 문제 화면                                            | 정답 시 효과                                          | 오답 시 화면                                          |
| ------------------------------------------------ | ------------------------------------------------ | ------------------------------------------------ |
| ![](/ScreenShots/Screenshot_20250513_100909.jpg) | ![](/ScreenShots/Screenshot_20250513_100933.jpg) | ![](/ScreenShots/Screenshot_20250513_101012.jpg) |

---

### Mission02 - 일과에 맞는 시계 선택

| 문제 화면                                            | 정답 시 효과                                          | 오답 시 화면                                          |
| ------------------------------------------------ | ------------------------------------------------ | ------------------------------------------------ |
| ![](/ScreenShots/Screenshot_20250513_101026.jpg) | ![](/ScreenShots/Screenshot_20250513_101038.jpg) | ![](/ScreenShots/Screenshot_20250513_101108.jpg) |

---

## 📁 프로젝트 구조 개요

해당 프로젝트는 아이들이 시간 개념을 쉽게 익힐 수 있도록 설계된 **시계탑 테마 학습 콘텐츠**입니다.
`GameManager`, `NarrationManager`, `SoundManager` 등의 공통 매니저 기반으로 작동하며,
아래 두 가지 주요 미션으로 나뉘어 구성됩니다:

1. **Mission01 - 아날로그 시계 바늘 맞추기**
2. **Mission02 - 디지털 시계로 하루 일과 시간 정하기**

---

## 📦 폴더 구조

```plaintext
/Assets
├── ClockScenes/
│   ├── ClockIntroScene.unity
│   ├── ClockMission1Scene.unity
│   └── ClockMission2Scene.unity
├── Scripts/
│   ├── ClockGameManager.cs
│   ├── ClockHandDragger.cs
│   ├── ClockAnswerChecker.cs
│   └── ClockNarrationManager.cs
```

---

## 🔁 실행 흐름 요약

### 🎬 1. 인트로 씬 (`IntroManager`)

* 웨이브 애니메이션 텍스트 출력
* `"터치하여 시작"` 반복 점멸
* ▶️ `Start` 클릭 시 → Mission01로 이동

---

### 🕰️ 2. Mission01 - 시계 바늘 맞추기

* `Mission01_DataManager`에서 문제 시작
* 아날로그 시계 드래그하여 정답 설정
* `TouchObjectDetector` + `WordEnter` 조합으로 판정
* 정답 시 이펙트 & `"정답이에요!"` 나레이션
* 3문제 완료 시 종료

---

### 📸 3. Mission02 - 일과 시간 맞추기

* 장면 이미지에 어울리는 시계 선택
* `Mission02_DataManager`, `Mission02_UIManager`로 분리된 흐름 관리
* 정답 선택 시 자동 다음 문제로 진행
* 6문제 완료 → 마무리 나레이션 출력

---

## 🔧 주요 클래스 설명

| 클래스                       | 역할 및 기능                       |
| ------------------------- | ----------------------------- |
| ✅ `GameManager`           | 전체 흐름 관리, 씬 전환, 터치 허용 여부 제어   |
| ✅ `Mission01_DataManager` | Mission01 문제 생성, 시계 배치, 진행 제어 |
| ✅ `Mission02_DataManager` | Mission02 문제 시퀀스 제어, 정답 설정    |
| ✅ `TouchObjectDetector`   | 드래그/터치 감지 → 정답 판정 분기 처리       |
| ✅ `WordEnter`             | 드래그 도착 후 정답 체크 (Mission01)    |
| ✅ `Mission02_Clock`       | 시계 선택 클릭 → 정답 체크 (Mission02)  |
| ✅ `NarrationManager`      | 나레이션 텍스트 + 오디오 동기화 처리         |
| ✅ `SoundManager`          | BGM / 효과음 / 나레이션 음성 관리        |
| ✅ `UIManager`             | 타이틀, 정답 텍스트, 효과 연출 담당         |

---

## 🧩 기타 보조 요소

* `ClockTowerCtrl`: 시계탑 효과 및 파티클 제어
* `CoroutineRunner`: 코루틴 실행 및 중복 방지 유틸리티
* `FloatObject`: 시계 오브젝트 부유 효과
* `SettingManager`: 볼륨 설정, 3D 모드, 재시작 기능 제공
