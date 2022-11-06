using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject earth;
    public TrailRenderer earthTrail;
    public GameObject moon;
    public TrailRenderer moonTrail;
    public GameObject spaceship;
    public TrailRenderer spaceshipTrail;
    public CameraLook cameraLook;
    public float earthCameraDistance;
    public float moonCameraDistance;
    public float spaceshipCameraDistance;
    public int recordingSize;
    public Slider timeline;
    public Text isPlayingUI;
    public Text speedUI;
    public GameObject helpPanel;
    public Text infoPanel;
    public float irlScale;

    private int currentCameraCenter;
    private Vector3[] moonPos;
    private Vector3[] spaceshipPos;
    private float[] temps;
    private float[] bats;
    private Vector3[] shipSpeed;
    private bool isPlaying = false;
    private int timer = 0;
    private int speed = 1;

    private void Start()
    {
        LoadRecordingData();

        currentCameraCenter = 0;
        cameraLook.Recenter(earth.transform, earthCameraDistance);
        timeline.minValue = 0;
        timeline.maxValue = recordingSize - 1;

        ResetRecording();
    }

    private void LoadRecordingData()
    {
        moonPos = new Vector3[recordingSize];
        spaceshipPos = new Vector3[recordingSize];
        temps = new float[recordingSize];
        bats = new float[recordingSize];
        shipSpeed = new Vector3[recordingSize];

        string moonXPath = "Assets/moon_Px.txt";
        string moonYPath = "Assets/moon_Py.txt";
        string moonZPath = "Assets/moon_Pz.txt";
        string spaceshipXPath = "Assets/spaceship_x.txt";
        string spaceshipYPath = "Assets/spaceship_y.txt";
        string spaceshipZPath = "Assets/spaceship_z.txt";
        string shipTemp = "Assets/spaceship info/temps.txt";
        string shipBat = "Assets/spaceship info/battery_level.txt";
        string shipX = "Assets/spaceship info/velocityX.txt";
        string shipY = "Assets/spaceship info/velocityY.txt";
        string shipZ = "Assets/spaceship info/velocityZ.txt";
        StreamReader moonXReader = new StreamReader(moonXPath);
        StreamReader moonYReader = new StreamReader(moonYPath);
        StreamReader moonZReader = new StreamReader(moonZPath);
        StreamReader spaceshipXReader = new StreamReader(spaceshipXPath);
        StreamReader spaceshipYReader = new StreamReader(spaceshipYPath);
        StreamReader spaceshipZReader = new StreamReader(spaceshipZPath);
        StreamReader shipTempReader = new StreamReader(shipTemp);
        StreamReader shipBatReader = new StreamReader(shipBat);
        StreamReader shipXReader = new StreamReader(shipX);
        StreamReader shipYReader = new StreamReader(shipY);
        StreamReader shipZReader = new StreamReader(shipZ);

        string moonX;
        string moonY;
        string moonZ;
        string spaceshipX;
        string spaceshipY;
        string spaceshipZ;
        string temp;
        string bat;
        string x;
        string y;
        string z;
        for (int i = 0; i < recordingSize; i++)
        {
            moonX = moonXReader.ReadLine();
            moonY = moonYReader.ReadLine();
            moonZ = moonZReader.ReadLine();
            spaceshipX = spaceshipXReader.ReadLine();
            spaceshipY = spaceshipYReader.ReadLine();
            spaceshipZ = spaceshipZReader.ReadLine();
            temp = shipTempReader.ReadLine();
            bat = shipBatReader.ReadLine();
            x = shipXReader.ReadLine();
            y = shipYReader.ReadLine();
            z = shipZReader.ReadLine();

            moonPos[i] = new Vector3(float.Parse(moonX), float.Parse(moonY), float.Parse(moonZ)) / irlScale;
            moonPos[i] += new Vector3(-84.82f, -22.79f, 0);
            spaceshipPos[i] = new Vector3(float.Parse(spaceshipX), float.Parse(spaceshipY), float.Parse(spaceshipZ))/ irlScale;
            temps[i] = float.Parse(temp);
            bats[i] = (int)float.Parse(bat);
            shipSpeed[i] = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
        }
    }

    private void Update()
    {
        GetInputs();
        timeline.value = timer;
        if (isPlaying) isPlayingUI.text = "Playing";
        else isPlayingUI.text = "Paused";
        speedUI.text = "Speed: " + speed*30 + " real mins/sec";
    }

    private void FixedUpdate()
    {
        if (isPlaying) AdvanceRecording();
    }

    private void GetInputs()
    {
        if (Input.GetButtonDown("Percpective"))
        {
            currentCameraCenter = (currentCameraCenter + 1) % 3;

            if (currentCameraCenter == 0) cameraLook.Recenter(earth.transform, earthCameraDistance);
            else if (currentCameraCenter == 1) cameraLook.Recenter(moon.transform, moonCameraDistance);
            else cameraLook.Recenter(spaceship.transform, spaceshipCameraDistance);

            FetchRecording(timer);

            earthTrail.Clear();
            moonTrail.Clear();
            spaceshipTrail.Clear();
        }

        if (Input.GetButtonDown("PlayPause"))
        {
            isPlaying = !isPlaying;
        }

        if (Input.GetButtonDown("Speed"))
        {
            if (speed == 1) speed = 15;
            else if (speed == 15) speed = 60;
            else speed = 1;
        }

        if (Input.GetButtonDown("ResetTimeline"))
        {
            ResetRecording();
        }

        if (Input.GetButtonDown("Help"))
        {
            helpPanel.SetActive(!helpPanel.activeSelf);
        }
    }

    private void ResetRecording()
    {
        timer = 0;
        FetchRecording(0);
    }

    private void AdvanceRecording()
    {
        if (timer + speed >= recordingSize - 1)
        {
            isPlaying = false;
            timer = recordingSize - 1;
            FetchRecording(timer);
        }
        else
        {
            timer += speed;
            FetchRecording(timer);
        }
    }

    private void FetchRecording(int index)
    {
        if (currentCameraCenter == 0)
        {
            earth.transform.position = Vector3.zero;
            moon.transform.position = moonPos[index];
            spaceship.transform.position = spaceshipPos[index];
        }
        else if (currentCameraCenter == 1)
        {
            earth.transform.position = -moonPos[index];
            moon.transform.position = Vector3.zero;
            spaceship.transform.position = spaceshipPos[index] - moonPos[index];
        }
        else
        {
            earth.transform.position = -spaceshipPos[index];
            moon.transform.position = moonPos[index] - spaceshipPos[index];
            spaceship.transform.position = Vector3.zero;
        }

        UpdadteShipInfo(index);
    }

    public void SetTimer(float value)
    {
        timer = (int)value;
        FetchRecording(timer);
    }

    public void UpdadteShipInfo(int index)
    {
        infoPanel.text = "ship info\r\n\r\n-----\r\nV(x): " + shipSpeed[index].x +
            "km/s\r\n\r\nV(y):\r\n" + shipSpeed[index].y +
            "km/s\r\n\r\nV(z):\r\n" + shipSpeed[index].z +
            "km/s\r\n\r\ntemp:\r\n" + temps[index] +
            "c\r\n\r\nbat:\r\n" + bats[index] + "%\r\n-----\r\n";
    }
}