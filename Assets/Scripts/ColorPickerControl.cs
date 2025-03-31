using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorPickerControl : MonoBehaviour
{
    public float currentHue, currentSat, currentVal;

    [SerializeField]
    private RawImage hueImage, satValImage, outputImage;

    [SerializeField]
    private Slider hueSlider;

    [SerializeField]
    private TMP_InputField hexInputField;

    private Texture2D hueTexture, svTexture, outputTexture;

    
    [SerializeField]
    MeshRenderer[] changeThisColour;

    private void Awake()
    {
        Color initialColor = changeThisColour[0].GetComponent<MeshRenderer>().sharedMaterial.color;

        Color.RGBToHSV(initialColor, out currentHue, out currentSat, out currentVal);

        CreateHueImage();

        CreateSVImage();

        CreateOutputImage();

        hueSlider.value = currentHue;

        UpdateOutputImage();
    }


    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1,16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        for(int i = 0; i<hueTexture.height; i++)
        {
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f));
        }
    
        hueTexture.Apply();

        hueImage.texture = hueTexture;
    }

    private void CreateSVImage()
    {
        svTexture = new Texture2D(16,16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        satValImage.texture = svTexture;
    }

    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1,16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for(int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }

        outputTexture.Apply();

        outputImage.texture = outputTexture;
    }


    private void UpdateOutputImage()
    {
        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for(int i = 0; i < outputTexture.height; i++)
        {
            outputTexture.SetPixel(0, i, currentColour);
        }

        outputTexture.Apply();

        hexInputField.text = ColorUtility.ToHtmlStringRGB(currentColour);

        for(int i = 0; i < changeThisColour.Length; i++)
        {
            changeThisColour[i].GetComponent<MeshRenderer>().sharedMaterial.color = currentColour;
        }

    }

    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;

        UpdateOutputImage();
    }

    public void UpdateSVImage()
    {
        currentHue = hueSlider.value;

        for(int y = 0; y < svTexture.height; y++)
        {
            for(int x = 0; x < svTexture.width; x++)
            {
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));
            }
        }

        svTexture.Apply();

        UpdateOutputImage();
    }

    public void OnTextInput()
    {
        if(hexInputField.text.Length < 6) {return; }
        
        Color newCol;

        if(ColorUtility.TryParseHtmlString("#" + hexInputField.text, out newCol))
            Color.RGBToHSV(newCol, out currentHue, out currentSat, out currentVal);

        hueSlider.value = currentHue;
        

        hexInputField.text = "";

        UpdateOutputImage();
    }
}
