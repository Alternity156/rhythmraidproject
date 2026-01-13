using UnityEngine;

public class TimeKeeper : MonoBehaviour
{
    public static TimeKeeper I { get; private set; }

    private void Awake()
    {
        I = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
