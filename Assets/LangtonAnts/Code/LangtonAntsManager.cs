using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LangtonAntsManager : MonoBehaviour
{
    public TMP_InputField inputRules;
    public TMP_InputField inputAmount;
    public Slider sliderSpeed;
    public Button buttonApply;

    [Space] // 
    public Vector2Int canvasScale = new Vector2Int(512, 512);

    public Renderer view;
    public Material dynamicMaterial;
    public float  timeStep = 0;

    private string _rules;
    private int _stepsPerFrame;
    private Vector3Int[] _ants;
    private int[,] _cells;
    private Texture2D _texture;
    private Color[] _colors;

    private void Start()
    {
        buttonApply.onClick.AddListener(HandleApply);
        sliderSpeed.onValueChanged.AddListener(HandleSpeedChanged);

        _cells = new int[canvasScale.x, canvasScale.y];
        
        _texture = new Texture2D(
            canvasScale.x,
            canvasScale.y,
            TextureFormat.RGBA32,
            0,
            true);
        _texture.filterMode = FilterMode.Point;

        dynamicMaterial = new Material(view.sharedMaterial);
        dynamicMaterial.mainTexture = _texture;
        view.sharedMaterial = dynamicMaterial;

        inputRules.text = "RL";
        inputAmount.text = "1";
        sliderSpeed.value = 1;
        _stepsPerFrame = 1;

        HandleApply();
    }

    private float _nextTime;

    void Update()
    {
        if (_nextTime < Time.time)
        {
            _nextTime += timeStep;
            
            for (int i = 0; i < _stepsPerFrame; i++)
            {
                DoStep();
            }

            _texture.Apply();
        }
    }

    void DoStep()
    {
        for (int i = 0; i < _ants.Length; i++)
        {
            int x = _ants[i].x;
            int y = _ants[i].y;

            var state = _cells[x, y];
            _cells[x, y] = (state + 1) % _rules.Length;
            _texture.SetPixel(x, y, _colors[state]);
            
            var rule = _rules[state];
            switch (rule)
            {
                case 'L':
                    _ants[i] = RotateLeft(_ants[i]);
                    break;
                case 'R':
                    _ants[i] = RotateRight(_ants[i]);
                    break;
            }

            _ants[i] = StepForward(_ants[i]);
        }
    }

    private Vector3Int StepForward(Vector3Int ant)
    {
        switch (ant.z)
        {
            case 0:
                ant.y++;
                break;
            case 1:
                ant.x++;
                break;
            case 2:
                ant.y--;
                break;
            case 3:
                ant.x--;
                break;
        }

        if (ant.x < 0) ant.x += canvasScale.x;
        if (ant.x >= canvasScale.x) ant.x -= canvasScale.x;
        if (ant.y < 0) ant.y += canvasScale.y;
        if (ant.y >= canvasScale.x) ant.y -= canvasScale.y;

        return ant;
    }

    private Vector3Int RotateRight(Vector3Int ant)
    {
        ant.z = (ant.z + 1) % 4;
        return ant;
    }

    private Vector3Int RotateLeft(Vector3Int ant)
    {
        ant.z = (ant.z + 3) % 4;
        return ant;
    }

    private int IndexOfColor(Color color)
    {
        for (int i = 0; i < _colors.Length; i++)
            if (_colors[i] == color)
                return i;
        return -1;
    }

    private void HandleSpeedChanged(float value)
    {
        _stepsPerFrame = (int)value;
    }

    private void HandleApply()
    {
        _rules = inputRules.text;
        _colors = new Color[_rules.Length];
        if (_colors.Length > 2)
            for (int i = 0; i < _colors.Length; i++)
                _colors[i] = Random.ColorHSV();
        _colors[0] = Color.white;
        _colors[_colors.Length - 1] = Color.black;

        for (int y = 0; y < canvasScale.y; y++)
        for (int x = 0; x < canvasScale.x; x++)
        {
            _texture.SetPixel(x, y, Color.white);
            _cells[x, y] = 0;
        }

        _texture.Apply();


        int amount = int.Parse(inputAmount.text);
        _ants = new Vector3Int[amount];
        int cx = canvasScale.x / 2, cy = canvasScale.y / 2;
        for (int i = 0; i < amount; i++)
            _ants[i] = new Vector3Int(cx, cy, 0);
    }
}