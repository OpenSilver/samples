# OpenSilver Samples

OpenSilver 애플리케이션 개발을 위한 샘플과 템플릿 모음입니다.

**XAML, C#, 데이터 바인딩, MVVM** 등 XAML 기반 요소들을 모듈화된 예제로 제공하며, 각 샘플은 독립적으로 구성되어 학습과 재사용이 쉽습니다.

## 🌐 라이브 데모

**[OpenSilver Hub](https://opensilverhub.azurewebsites.net)** (임시)에서 모든 예제를 바로 확인할 수 있습니다.

- **실시간 실행** - 브라우저에서 바로 샘플 동작 확인
- **인터랙티브 테스트** - 컨트롤 조작 및 동작 방식 학습  
- **소스코드 보기** - XAML과 C# 코드 즉시 확인
- **구현 아이디어** - 다양한 UI 패턴과 구현 방법 참고

## 🚀 개발 환경 설정

### 1. 레포지터리 클론
```bash
git clone https://github.com/opensilver/samples
cd samples
```

### 2. Visual Studio 템플릿 설치
```bash
cd templates
install_templates.bat
```

설치되는 템플릿:
- **Showcase Template (OpenSilver)** - 프로젝트 템플릿
- **Showcase Content (OpenSilver)** - 아이템 템플릿
- **Showcase Item (OpenSilver)** - 아이템 템플릿

### 3. 솔루션 열기
```bash
cd ../src
# OpenSilverSample.sln을 Visual Studio에서 열기
```

## 🏗️ 프로젝트 구조

```
samples/
├── templates/
│   ├── install_templates.bat
│   └── vs-templates/
└── src/
    ├── OpenSilverSample.sln     # 메인 솔루션
    ├── OpenSilverSample/        # 샘플 프로젝트
    │   └── MainPage.xaml        # 메인 쇼케이스 페이지
    └── OpenSilverSample.Browser/ # 실행 프로젝트
```

## 🔧 새 샘플 만들기

### 새 프로젝트 추가
솔루션 우클릭 → 추가 → 새 프로젝트 → "Showcase Template (OpenSilver)" 선택

![새 프로젝트 추가](https://github.com/user-attachments/assets/08c6eaf8-be4d-4c25-84b2-7368be21f7ed)

### 새 아이템 추가
프로젝트 우클릭 → 추가 → 새 항목 → "Showcase Content" 또는 "Showcase Item" 선택

![새 아이템 추가](https://github.com/user-attachments/assets/7f818fcb-9bdd-44e3-8a4d-b28604711fd8)

### 샘플 통합 및 테스트
1. `MainPage.xaml`에 새 TabItem 추가:
```xml
<TabItem Header="MySample">
    <mysample:MySampleContent IsMenuPanelVisible="True" DefaultSelectedItemName="Basic"/>
</TabItem>
```

2. `OpenSilverSample.Browser` 실행해서 테스트

## 📋 기존 샘플들

- **기본 컨트롤**: Button, ToggleButton, Slider, CheckBox, RadioButton
- **데이터 컨트롤**: ComboBox, ListBox, TextBox, ProgressBar
- **레이아웃**: Grid, StackPanel, WrapPanel, Border
- **고급 컴포넌트**: AnimatedNavigationBar, AdaptiveColumnsPanel, StaggeredPanel
- **패턴**: MVVM, Binding, DataContext, Resource

## 🤝 기여

1. 레포지터리 포크
2. 로컬에 클론 후 템플릿 설치
3. 새 샘플 개발
4. 테스트 완료 후 Pull Request

## 🔗 관련 링크

- [OpenSilver 공식 사이트](https://opensilver.net/)
- [OpenSilver GitHub](https://github.com/opensilver/opensilver)
- [커뮤니티 포럼](https://forums.opensilver.net/)
