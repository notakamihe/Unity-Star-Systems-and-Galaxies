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

    public StarUI starUI;
    public PlanetUI planetUI;
    public MoonUI moonUI;
    public StarSystemUI starSystemUI;
    public BeltUI beltUI;
    public WhiteDwarfUI whiteDwarfUI;
    public NeutronStarUI neutronStarUI;
    public BlackHoleUI blackHoleUI;

    public void SetUI(UI ui)
    {
        if (ui == this.starUI)
        {
            this.starUI.parent.SetActive(true);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.neutronStarUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(false);
        }
        else if (ui == this.planetUI)
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(true);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.neutronStarUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(false);
        }
        else if (ui == this.moonUI)
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(true);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.neutronStarUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(false);
        }
        else if (ui == this.starSystemUI)
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(true);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(false);
        }
        else if (ui == this.whiteDwarfUI)
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(true);
            this.neutronStarUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(false);
        }
        else if (ui == this.neutronStarUI)
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.neutronStarUI.parent.SetActive(true);
            this.blackHoleUI.parent.SetActive(false);
        }
        else if (ui == this.blackHoleUI)
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.neutronStarUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(true);
        }
        else if (ui == this.beltUI)
        {
            this.beltUI.parent.SetActive(true);
        }
        else
        {
            this.starUI.parent.SetActive(false);
            this.planetUI.parent.SetActive(false);
            this.moonUI.parent.SetActive(false);
            this.starSystemUI.parent.SetActive(false);
            this.beltUI.parent.SetActive(false);
            this.whiteDwarfUI.parent.SetActive(false);
            this.neutronStarUI.parent.SetActive(false);
            this.blackHoleUI.parent.SetActive(false);
        }
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