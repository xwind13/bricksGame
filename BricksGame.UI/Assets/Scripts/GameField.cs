using Assets.Scripts;
using BricksGame.Logic;
using BricksGame.Logic.Matrix;
using BricksGame.Logic.Models;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameField : MonoBehaviour
{
    [SerializeField] private SideField _sideFieldPrefub;
    [SerializeField] private MainField _mainFieldPrefub;

    [SerializeField] private Text _scoreText;
    [SerializeField] private AudioSource _laserSound;
    [SerializeField] private AudioSource _boomSound;
    [SerializeField] private AudioSource _glassSound;
    [SerializeField] private AudioSource _hrumSound;
    [SerializeField] private AudioSource _congratulationSound;
    [SerializeField] private AudioMixer _audioSource;

    [SerializeField] private GameObject _gameOverPanel;

    private const float Offset = BrickSetting.TileSize / 2;
    private const int HalfFieldSize = 5;

    private bool _isBlocked = false;
    private Scene _gameSceneLogic;

    // Start is called before the first frame update
    void Start()
    {
        var settings = FieldSetting.getDefault();
        settings.HorzDimension = BrickSetting.FieldSize;
        settings.VertDimension = BrickSetting.FieldSize;
        settings.SideDimension = BrickSetting.SideFieldSize;

        var sliderValue = PlayerPrefs.GetFloat("Sound");
        _audioSource.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);

        _gameSceneLogic = new Scene(settings);

        _gameSceneLogic.ScoreUpdated += ScoreUpdated;
        _gameSceneLogic.CombinationDestroyed += CombinationDestroyed;
        _gameSceneLogic.GameOver += GameOver;
        _gameSceneLogic.GameWon += GameWon;

        InstantiateMainField(_gameSceneLogic.MainFieldMatrix, _gameSceneLogic.MovingSquare);

        InstantiateTopSideField(_gameSceneLogic.GetSideMatrix(Side.Top));
        InstantiateLeftSideField(_gameSceneLogic.GetSideMatrix(Side.Left));
        InstantiateBottomSideField(_gameSceneLogic.GetSideMatrix(Side.Bottom));
        InstantiateRightSideField(_gameSceneLogic.GetSideMatrix(Side.Right));
    }

    private void GameWon()
    {
        _congratulationSound.Play();
    }

    private void GameOver()
    {
        _glassSound.Play();
        _gameOverPanel.SetActive(true);
        _isBlocked = true;
    }

    private void CombinationDestroyed(System.Collections.Generic.IEnumerable<IMainFieldSquare> obj)
    {
        _boomSound.Play();
    }

    private void ScoreUpdated(int score)
    {
        _scoreText.text = score.ToString();
    }

    private SideField InstantiateTopSideField(IMatrix<ISquare> matrix)
    {
        var position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize + Offset,
            HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(Side.Top, matrix, position);
    }

    private SideField InstantiateLeftSideField(IMatrix<ISquare> matrix)
    {
        var position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize - BrickSetting.SideFieldSize * BrickSetting.TileSize + Offset,
            -HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(Side.Left, matrix, position);
    }

    private SideField InstantiateBottomSideField(IMatrix<ISquare> matrix)
    {
        var position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize + Offset,
            -HalfFieldSize * BrickSetting.TileSize - BrickSetting.SideFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(Side.Bottom, matrix, position);
    }

    private SideField InstantiateRightSideField(IMatrix<ISquare> matrix)
    {
        var position = new Vector3(
            HalfFieldSize * BrickSetting.TileSize + Offset,
            -HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return InstantiateSideField(Side.Right, matrix, position);
    }

    private SideField InstantiateSideField(Side side, IMatrix<ISquare> matrix, Vector3 position)
    {
        var sideField = Instantiate(_sideFieldPrefub, gameObject.transform);
        sideField.Side = side;
        sideField.Matrix = matrix;
        sideField.transform.position = position;
        sideField.ClickEventHandler += SideFieldClickEventHandler;

        return sideField;
    }

    private void SideFieldClickEventHandler(object sender, Assets.Scripts.Events.SideFieldClickEventArgs e)
    {
        if(!_isBlocked && _gameSceneLogic.ThrowSquare(e.Side, e.PosIdx))
        {
            _laserSound.Play();
        }
    }

    private MainField InstantiateMainField(IMatrix<IMainFieldSquare> mainMatrix, MovingSquare movingSquare)
    {
        var mainField = Instantiate(_mainFieldPrefub, gameObject.transform);
        mainField.Matrix = mainMatrix;
        mainField.MovingSquare = movingSquare;
        mainField.transform.position = new Vector3(
            -HalfFieldSize * BrickSetting.TileSize + Offset, 
            -HalfFieldSize * BrickSetting.TileSize + Offset, 0);

        return mainField;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !_isBlocked)
        {
            if (_gameSceneLogic.BackToPreviousState())
            {
                _hrumSound.Play();
            }
        }
    }

    public void Block()
    {
        _isBlocked = true;
    }

    public void Unblock()
    {
        _isBlocked = false;
    }

    public void StartNewGame()
    {
        _gameSceneLogic.Restart();
        _isBlocked = false;
    }
}
