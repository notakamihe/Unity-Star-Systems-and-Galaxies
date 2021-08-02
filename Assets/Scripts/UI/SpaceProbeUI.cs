using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class SpaceProbeUI : MonoBehaviour
{
    public Text precedingPlanet;
    public Text succeedingPlanet;
    public Image galaticSpeedProgressBar;
    public Text nearestStar;

    public StarUI starUI;
    public PlanetUI planetUI;
    public MoonUI moonUI;
    public StarSystemUI starSystemUI;
    public BeltUI beltUI;
    public WhiteDwarfUI whiteDwarfUI;
    public NeutronStarUI neutronStarUI;
    public BlackHoleUI blackHoleUI;
    public GalaxyUI galaxyUI;

    public void SetUI(UI ui)
    {
        this.starUI.parent.SetActive(false);
        this.planetUI.parent.SetActive(false);
        this.moonUI.parent.SetActive(false);
        this.starSystemUI.parent.SetActive(false);
        this.beltUI.parent.SetActive(false);
        this.whiteDwarfUI.parent.SetActive(false);
        this.neutronStarUI.parent.SetActive(false);
        this.blackHoleUI.parent.SetActive(false);
        this.galaxyUI.parent.SetActive(false);

        if (ui == this.starUI)
            this.starUI.parent.SetActive(true);
        else if (ui == this.planetUI)
            this.planetUI.parent.SetActive(true);
        else if (ui == this.moonUI)
            this.moonUI.parent.SetActive(true);
        else if (ui == this.starSystemUI)
            this.starSystemUI.parent.SetActive(true);
        else if (ui == this.whiteDwarfUI)
            this.whiteDwarfUI.parent.SetActive(true);
        else if (ui == this.neutronStarUI)
            this.neutronStarUI.parent.SetActive(true);
        else if (ui == this.blackHoleUI)
            this.blackHoleUI.parent.SetActive(true);
        else if (ui == galaxyUI)
            this.galaxyUI.parent.SetActive(true);
        else if (ui == this.beltUI)
            this.beltUI.parent.SetActive(true);
    }
}

[System.Serializable]
public class UI
{
    public GameObject parent;
}

[System.Serializable]
public class StarUI : UI
{
    public Text name;
    public Text distance;
    public Text temperature;
    public Text radius;
    public Text mass;
    public Text luminosity;
    public Text type;

    public void SetType(StarType starType)
    {
        switch (starType)
        {
            case StarType.RedDwarf:
                this.type.text = "RED DWARF";
                this.type.color = Color.red;
                break;
            case StarType.YellowDwarf:
                this.type.text = "YELLOW DWARF";
                this.type.color = Color.yellow;
                break;
            case StarType.AMainSequence:
                this.type.text = "A-TYPE MAIN SEQUENCE";
                this.type.color = Color.white;
                break;
            case StarType.BMainSequence:
                this.type.text = "B-TYPE MAIN SEQUENCE";
                this.type.color = new Color(0.65f, 0.65f, 1.0f);
                break;
            case StarType.Giant:
                this.type.text = "GIANT";
                this.type.color = Color.red + Color.green * 0.3f;
                break;
            case StarType.OMainSequence:
                this.type.text = "O-TYPE MAIN SEQUENCE";
                this.type.color = Color.blue;
                break;
            case StarType.BlueSupergiant:
                this.type.text = "BLUE SUPERGIANT";
                this.type.color = Color.blue;
                break;
            case StarType.RedSupergiant:
                this.type.text = "RED SUPERGIANT";
                this.type.color = Color.red;
                break;
        }
    }
}

[System.Serializable]
public class PlanetUI : UI
{
    public Text name;
    public Text distance;
    public Text temperature;
    public Text radius;
    public Text mass;
    public Text axialTilt;
    public Text orbit;
    public Text habitablity;
}

[System.Serializable]
public class MoonUI : UI
{
    public Text name;
    public Text distance;
    public Text temperature;
    public Text radius;
    public Text mass;
    public Text inclination;
    public Text orbit;
    public Text habitablity;
}

[System.Serializable]
public class StarSystemUI : UI
{
    public Text name;
    public Text numPlanets;
    public Text numDwarfPlanets;
    public Text radius;
    public Text distance;
}

[System.Serializable]
public class BeltUI : UI
{
    public Text name;
    public Text distanceFromStar;
    public Text thickness;
}

[System.Serializable]
public class WhiteDwarfUI : UI
{
    public Text distance;
    public Text mass;
    public Text temperature;
}

[System.Serializable]
public class NeutronStarUI : UI
{
    public Text distance;
    public Text mass;
    public Text temperature;
    public Text hertz;
}

[System.Serializable]
public class BlackHoleUI : UI
{
    public Text distance;
    public Text mass;
    public Text hertz;
}

[System.Serializable]
public class GalaxyUI : UI
{
    public Text name;
    public Text diameter;
    public Text distance;
    public Text numStars;
    public Text shape;

    public void SetShape(GalaxyShape shape)
    {
        switch (shape)
        {
            case GalaxyShape.Spiral:
                this.shape.text = "SPIRAL";
                break;
            case GalaxyShape.Elliptical:
                this.shape.text = "ELLIPTICAL";
                break;
        }
    }
}