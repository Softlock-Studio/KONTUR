using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TVLightEffect : MonoBehaviour
{
    [Header("Light Settings")]
    public Light tvLight;
    public float baseIntensity = 30f;
    public float flickerSpeed = 3f;
    public float flickerAmount = 0.2f;
    public float timeNextFlicker = 0.7f;

    [Header("Color Settings")]
    public float colorChangeInterval = 3f; // Интервал смены цвета
    public float colorSmoothness = 2f; // Плавность перехода

    [Header("Color Palettes")]
    public List<Color> tvColors = new List<Color>()
    {
        Color.white,      // Нейтральный
        Color.red,        // Красный
        Color.green,      // Зеленый
        Color.blue,       // Синий
    };

    public enum ColorMode
    {
        Random,        // Случайный цвет
        Sequential,    // По порядку
        SceneBased,    // В зависимости от "сцены"
        Pulse,         // Пульсирующий переход
        Rainbow        // Радуга
    }
    public ColorMode colorMode = ColorMode.Random;

    [Header("References")]
    public MeshRenderer screenQuad; // Экран телевизора (невидимый)
    public Material screenMaterial;

    private float currentIntensity;
    private Color currentColor;
    private Color targetColor;
    private float colorTimer;
    private int currentColorIndex = 0;
    private Coroutine flickerCoroutine;

    void Start()
    {
        if (tvLight == null)
            tvLight = GetComponent<Light>();

        // Настройка материала экрана
        if (screenQuad != null)
        {
            screenMaterial = screenQuad.material;
            screenMaterial.EnableKeyword("_EMISSION");
            screenQuad.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        // Устанавливаем начальный цвет
        currentColor = tvColors[0];
        targetColor = tvColors[0];
        tvLight.color = currentColor;

        // Запускаем эффекты
        StartCoroutine(ColorChangeRoutine());
        StartCoroutine(FlickerRoutine());
    }

    void Update()
    {
        // Плавный переход цвета
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorSmoothness);
        tvLight.color = currentColor;

        // Обновляем эмиссию экрана
        if (screenMaterial != null)
        {
            screenMaterial.SetColor("_EmissionColor", currentColor * 3f);
        }
    }

    IEnumerator ColorChangeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(colorChangeInterval);

            // Выбираем новый цвет
            switch (colorMode)
            {
                case ColorMode.Random:
                    targetColor = tvColors[Random.Range(0, tvColors.Count)];
                    break;

                case ColorMode.Sequential:
                    currentColorIndex = (currentColorIndex + 1) % tvColors.Count;
                    targetColor = tvColors[currentColorIndex];
                    break;

                case ColorMode.Rainbow:
                    // Радуга через HSV
                    float hue = (Time.time * 0.1f) % 1f;
                    targetColor = Color.HSVToRGB(hue, 1f, 1f);
                    break;

                case ColorMode.Pulse:
                    StartCoroutine(PulseColor());
                    break;
            }
        }
    }

    IEnumerator PulseColor()
    {
        // Пульсация между двумя цветами
        Color startColor = currentColor;
        Color endColor = tvColors[Random.Range(0, tvColors.Count)];
        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            targetColor = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        targetColor = endColor;
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Разные паттерны мерцания
            float flickerPattern = Random.Range(0, 100);

            if (flickerPattern < 30) // Нормальное мерцание
            {
                float flicker = 1f + Random.Range(-flickerAmount, flickerAmount);
                tvLight.intensity = baseIntensity * flicker;
                yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));
            }
            else if (flickerPattern < 40) // Короткое выключение
            {
                tvLight.intensity = 0;
                yield return new WaitForSeconds(0.05f);
                tvLight.intensity = baseIntensity;
                yield return new WaitForSeconds(0.1f);
            }
            else if (flickerPattern < 50) // Яркая вспышка
            {
                tvLight.intensity = baseIntensity * 2f;
                yield return new WaitForSeconds(0.05f);
                tvLight.intensity = baseIntensity;
                yield return new WaitForSeconds(0.2f);
            }
            else // Обычное состояние
            {
                tvLight.intensity = baseIntensity * (1f + Mathf.Sin(Time.time * flickerSpeed) * flickerAmount);
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(timeNextFlicker);
        }
    }

    // Метод для ручной смены цвета
    public void SetColor(Color newColor)
    {
        targetColor = newColor;
    }

    // Метод для смены цвета по индексу
    public void SetColorByIndex(int index)
    {
        if (index >= 0 && index < tvColors.Count)
        {
            targetColor = tvColors[index];
        }
    }

    // Метод для смены цвета по имени
    public void SetColorByName(string colorName)
    {
        switch (colorName.ToLower())
        {
            case "red": targetColor = Color.red; break;
            case "green": targetColor = Color.green; break;
            case "blue": targetColor = Color.blue; break;
            case "white": targetColor = Color.white; break;
            case "yellow": targetColor = Color.yellow; break;
            case "cyan": targetColor = Color.cyan; break;
            case "magenta": targetColor = Color.magenta; break;
            case "orange": targetColor = new Color(1f, 0.5f, 0f); break;
            case "purple": targetColor = new Color(0.5f, 0f, 1f); break;
        }
    }
}

// Дополнительный скрипт для создания "сцен" с разными цветами
public class TVSceneController : MonoBehaviour
{
    public TVLightEffect tvEffect;

    [System.Serializable]
    public class TVScene
    {
        public string sceneName;
        public Color sceneColor;
        public float intensityMultiplier = 1f;
        public float flickerAmount = 0.2f;
    }

    public List<TVScene> scenes = new List<TVScene>();
    private int currentSceneIndex = 0;

    void Start()
    {
        // Создаем сцены с разными цветами
        scenes.Add(new TVScene { sceneName = "News", sceneColor = new Color(0.8f, 0.8f, 1f), intensityMultiplier = 1f });
        scenes.Add(new TVScene { sceneName = "Action", sceneColor = Color.red, intensityMultiplier = 1.2f, flickerAmount = 0.3f });
        scenes.Add(new TVScene { sceneName = "Nature", sceneColor = Color.green, intensityMultiplier = 0.8f });
        scenes.Add(new TVScene { sceneName = "Night", sceneColor = new Color(0.2f, 0.2f, 0.8f), intensityMultiplier = 0.5f });
        scenes.Add(new TVScene { sceneName = "Horror", sceneColor = new Color(0.5f, 0f, 0f), flickerAmount = 0.5f });
        scenes.Add(new TVScene { sceneName = "Music", sceneColor = new Color(1f, 0.2f, 0.8f), intensityMultiplier = 1.3f });

        // Запускаем авто-смену сцен
        StartCoroutine(SceneRoutine());
    }

    IEnumerator SceneRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));

            currentSceneIndex = (currentSceneIndex + 1) % scenes.Count;
            ApplyScene(scenes[currentSceneIndex]);
        }
    }

    public void ApplyScene(TVScene scene)
    {
        tvEffect.SetColor(scene.sceneColor);
        tvEffect.baseIntensity = 30f * scene.intensityMultiplier;
        tvEffect.flickerAmount = scene.flickerAmount;

        Debug.Log($"TV Scene: {scene.sceneName} - Color: {scene.sceneColor}");
    }

    public void NextScene()
    {
        currentSceneIndex = (currentSceneIndex + 1) % scenes.Count;
        ApplyScene(scenes[currentSceneIndex]);
    }
}