using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ControlsUI : MonoBehaviour {

    [SerializeField] private Container[] graphics;

    public void OnDeviceChanged(PlayerInput input) {
        bool isKeyboard = input.currentControlScheme == "Keyboard&Mouse";
        foreach(Container container in graphics) {
            container.targetGraphic.sprite = isKeyboard ? container.keyboardSprite : container.gamepadSprite;
        }
    }

    [System.Serializable]
    public class Container {
        public Image targetGraphic;
        public Sprite keyboardSprite;
        public Sprite gamepadSprite;
    }

}
