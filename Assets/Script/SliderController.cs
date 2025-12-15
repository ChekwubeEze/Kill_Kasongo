using UnityEngine;
using UnityEngine.UI;
public class SliderController : MonoBehaviour
{
    public void OnSlideValueChange(float value)
    {
        Debug.Log("The slider value is: " + value);
    }
}
