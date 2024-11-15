using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    // Informacoes da tela
    [SerializeField] Text score; // Pontuacao atual
    [SerializeField] Text gameover; // Fim de jogo
    [SerializeField] Text startgame; // Começo do jogo
    [SerializeField] Text highscore; // Pontuacao maxima

    [SerializeField] SpriteRenderer[] colors; // Array com as cores
    [SerializeField] float twinkle; // Tempo que a luz ficara acesa
    [SerializeField] float sleepTwinkle;
    [SerializeField] List<int> activeSequence; // Lista para a sequencia ativa
    
    [SerializeField] AudioSource[] sounds; // Array com os sons das cores
    [SerializeField] AudioSource correct; // Som pra quando acerta
    [SerializeField] AudioSource incorrect; // Som pra quando erra


    public bool playing; // Indica que o jogando esta colocando a sequencia
    
    private int sequencePosition; // Indice da lista de sequencia ativa

    private int input; // Contagem do input do jogador
    private int colorSelect; // Cor aleatoria selecionada para a sequencia
    private float stayLitCounter; // Contador para manter aceso
    private float sleepCounter;

    private bool dark; // Indica que esta apagado
    private bool lightUp; // Indica que esta aceso

    private float countdown = 5.15f; // Contador de iniciar o jogo
    private float timebetweenlights = 0.5f; // Tempo para os sons nao ficarem "Continuos"
    private bool running; // Indica que o jogo esta rodando e habilita os botoes
    private float timerbetweensequence = 1.2f; // Tempo entre a resposta do usuario e a nova sequencia

    public bool canInteract; // Indica se os botoes podem ser apertados

    void Start()
    {
        if (!PlayerPrefs.HasKey("HiScore"))
        {
            PlayerPrefs.SetInt("HiScore", 0);
        }

        highscore.text = "Highscore: " + PlayerPrefs.GetInt("HiScore");
    }

    void Update()
    {
        if (lightUp)
        {
            stayLitCounter -= Time.deltaTime;
            if (stayLitCounter < 0)
            {
                colors[activeSequence[sequencePosition]].color = new Color(colors[activeSequence[sequencePosition]].color.r, colors[activeSequence[sequencePosition]].color.g, colors[activeSequence[sequencePosition]].color.b, 0.2392157f);
                sounds[activeSequence[sequencePosition]].Stop();
                lightUp = false;
                
                dark = true;
                sleepCounter = sleepTwinkle;
                timebetweenlights = 0.5f;

                sequencePosition++;
            }
        }

        if (dark)
        {
            sleepCounter -= Time.deltaTime;

            if (sequencePosition >= activeSequence.Count)
            {
                dark = false;
                playing = true;
            }
            else
            {
                if (sleepCounter < 0)
                {
                    timebetweenlights -= Time.deltaTime;
                    if (timebetweenlights < 0)
                    {
                        colors[activeSequence[sequencePosition]].color = new Color(colors[activeSequence[sequencePosition]].color.r, colors[activeSequence[sequencePosition]].color.g, colors[activeSequence[sequencePosition]].color.b, 1f);
                        sounds[activeSequence[sequencePosition]].Play();
                        stayLitCounter = twinkle;
                        lightUp = true;
                        dark = false;
                    }
                }
            }
        }

        if (!running)
        {
            startgame.text = "The game starts at:\n" + Mathf.FloorToInt(countdown % 60);
            countdown -= Time.deltaTime;
            canInteract = false;

            if (countdown <= 0)
            {
                StartGame();
                canInteract = true;
                countdown = 0;
                startgame.text = "";
                running = true;
            }
        }
    }

    public void StartGame()
    {
        score.text = "Score: 0";
        highscore.text = "Highscore: " + PlayerPrefs.GetInt("HiScore");
        gameover.text = "";
        activeSequence.Clear();
        NewSequence();
    }

    private void NewSequence()
    {
        sequencePosition = 0;
        input = 0;

        colorSelect = Random.Range(0, 4);

        activeSequence.Add(colorSelect);

        colors[activeSequence[sequencePosition]].color = new Color(colors[activeSequence[sequencePosition]].color.r, colors[activeSequence[sequencePosition]].color.g, colors[activeSequence[sequencePosition]].color.b, 1f);
        sounds[activeSequence[sequencePosition]].Play();
        stayLitCounter = twinkle;
        lightUp = true;
    }

    public void ColorSelected(int button)
    {
        if (playing)
        {
            if (activeSequence[input] == button)
            {
                Debug.Log("Correct");
                input++;

                if (input >= activeSequence.Count)
                {
                    if (activeSequence.Count > PlayerPrefs.GetInt("HiScore"))
                    {
                        PlayerPrefs.SetInt("HiScore", activeSequence.Count);
                    }

                    score.text = "Score: " + activeSequence.Count;
                    highscore.text = "Highscore: " + PlayerPrefs.GetInt("HiScore");

                    correct.Play();
                    playing = false;
                    StartCoroutine(WaitBetweenSequences()); // Uso de Coroutine para dar o tempo entre o input do jogador e a nova sequencia
                }
            }
            else
            {
                Debug.Log("Wrong");
                gameover.text = "Game Over";
                incorrect.Play();
                playing = false;
                running = false;
                countdown = 5.15f;
            }
        }
    }

    IEnumerator WaitBetweenSequences()
    {
        yield return new WaitForSeconds(timerbetweensequence);

        NewSequence();
    }
}
