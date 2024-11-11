using UnityEngine;

public class CheckInput : MonoBehaviour
{
    public int selected;
    
    private SpriteRenderer sprite;
    private MainScript script;
    private AudioSource sound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        script = FindFirstObjectByType<MainScript>();
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        if (!script.canInteract || !script.playing) { return; }
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        sound.Play();
    }

    private void OnMouseUp()
    {
        if (!script.canInteract || !script.playing) { return; }
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        script.ColorSelected(selected);
        sound.Stop();
    }
}
