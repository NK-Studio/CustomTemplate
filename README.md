# CustomTemplate UI

# 설치방법

```` 
https://github.com/NK-Studio/unity-scene-reference.git#UPM
````
다음 UPM 주소를 Unity Package Manager에 +버튼을 누르고 추가합니다.

# 사용법

``` C#
using UnityEngine;

public class Demo : MonoBehaviour
{
    public SceneReference TargetScene;
    
    private void Start()
    {
        // Gets the address of the scene.
        Debug.Log(TargetScene.Path);
        
        // Gets the name of the scene.
        Debug.Log(TargetScene.Name);
    }
}
```