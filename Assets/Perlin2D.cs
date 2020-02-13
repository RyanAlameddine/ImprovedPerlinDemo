using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Perlin2D : MonoBehaviour
{
    [SerializeField]
    Material material;
    [SerializeField]
    Slider scaleSlider;
    [SerializeField]
    Slider XSlider;
    [SerializeField]
    Slider YSlider;
    [SerializeField]
    Slider ZSlider;
    [SerializeField]
    Slider PersistSlider;
    [SerializeField]
    Slider LacunSlider;

    [SerializeField]
    Toggle twoDToggle;
    [SerializeField]
    Toggle randomToggle;

    [SerializeField]
    InputField octaveField;

    
    float scale = 1.1f;
    bool twoD = false;
    Texture2D texture;

    [SerializeField]
    int sideLength = 100;

    [SerializeField]
    bool autoGenerate = true;

    float X = 0;
    float Y = 0;
    float Z = 0;

    int octaves = 1;
    float persistance = .3f;
    float lacunarity = .3f;

    bool random = true;
    System.Random rand = new System.Random(1);
    Vector2[] octaveOffsets;

    private void Start()
    {
        texture = new Texture2D(sideLength, sideLength);
        material.mainTexture = texture;
        Regen();
    }

    private void FixedUpdate()
    {
        if (autoGenerate)
        {
            Regen();
        }
    }



    public void Regen()
    {
        random = randomToggle.isOn;
        int seed = (int)(X + Y + Z + scale);
        if (random)
        {
            rand = new System.Random(seed);
        }
        else
        {
            rand = new System.Random(2);
        }
        twoD = twoDToggle.isOn;
        scale = scaleSlider.value;
        X = XSlider.value;
        Y = YSlider.value;
        Z = ZSlider.value;

        persistance = PersistSlider.value;
        lacunarity = LacunSlider.value;
        
        if(octaveField.text != "")
        {
            octaves = int.Parse(octaveField.text);
            octaveField.text = octaves + "";
        }
        octaveOffsets = new Vector2[octaves];

        if (texture.width != sideLength)
        {
            texture = new Texture2D(sideLength, sideLength);
            material.mainTexture = texture;
        }
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = (float)rand.NextDouble() * 100 + .12f;
            float offsetY = (float)rand.NextDouble() * 100 + .12f;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        if (twoD)
        {
            for (int y = 0; y < sideLength; y++)
            {
                for (int x = 0; x < sideLength; x++)
                {
                    texture.SetPixel(x, y, Color.Lerp(Color.black, Color.white, (generate(x / scale, y / scale) + 1)/2));
                }
            }
        }
        else
        {
            for (int y = 0; y < 50; y++)
            {
                for (int x = 0; x < sideLength; x++)
                {
                    if ((int) (((generate(x / scale) + 1) / 2) * 50) == y) {
                        texture.SetPixel(x, y + sideLength/2 + -25, Color.black);
                    }
                    else
                    {
                        texture.SetPixel(x, y + sideLength / 2 + -25, Color.white);
                    }
                }
            }
        }
        //texture = new Texture2D(sideLength, sideLength);
        texture.Apply();
        //material.mainTexture =
    }

    public float generate(float x, float y)
    {
        return random ? (float)rand.NextDouble()*2-1 : perlin(x + X, y + Y, Z);
    }
    public float generate(float x)
    {
        return random ? (float)rand.NextDouble()*2-1 : perlin(x + X, Y, Z);
    }

    private float perlin(float x, float y, float z)
    {
        float noiseHeight = 0; 
        float amplitutde = 1;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            
            float perlinValue = Perlin.perlin(x * frequency + octaveOffsets[i].x, y * frequency + octaveOffsets[i].y, z * frequency);
            noiseHeight += perlinValue * amplitutde;

            amplitutde *= persistance;
            frequency *= lacunarity;
        }
        return noiseHeight;

    }
}
