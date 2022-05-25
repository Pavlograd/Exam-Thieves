using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum CharacterType : int
{
    Artiste = 0,
    Influenceuse,
    Jock,
    Nerd,
    NumberOfTypes
}

public class PlayerCharacter : NetworkBehaviour
{
    private bool hasPower = true;
    private CharacterType _characterType;
    public bool flash = false;
    public float hackingProgression = 0.0f;
    [SerializeField] GameObject trap;
    [SerializeField] SFXManager _SFXManager;
    [SerializeField] PlayerCatched pCatched;

    public bool GetHasPower()
    {
        return hasPower;
    }

    public void SetCharacterType(CharacterType type)
    {
        _characterType = type;
    }

    public CharacterType GetCharacterType()
    {
        return _characterType;
    }

    public bool Refill(CharacterType type)
    {
        if (type == _characterType && !hasPower)
        {
            hasPower = true;
            return true;
        }
        return false;
    }

    void UsePowerArtiste()
    {
        Debug.Log("Artiste");

        _SFXManager.ChangeState("PaintSpill");
        GameObject go = Instantiate(trap, transform.position, Quaternion.Euler(0, Random.Range(0, 360.0f), 0));
    }

    void UsePowerInfluenceuse()
    {
        Debug.Log("Influen");
        _SFXManager.ChangeState("Flash");
        GameObject[] guards = GameObject.FindGameObjectsWithTag("garde");
        GameObject[] profs = GameObject.FindGameObjectsWithTag("Professeur");

        foreach (GameObject guard in guards)
        {
            if (Vector3.Distance(guard.transform.position, transform.position) < 5.0f)
            {
                AiAgent agent = guard.GetComponent<AiAgent>();
                agent.stateMachine.ChangeState(AiStateId.Stun);
            }
        }
        foreach (GameObject prof in profs)
        {
            if (Vector3.Distance(prof.transform.position, transform.position) < 5.0f)
            {
                AiProfesseur professeur = prof.GetComponent<AiProfesseur>();
                professeur.professeurStateMachine.ChangeState(AiProfesseurStateId.Stun);
            }
        }

        flash = true;
    }

    void UsePowerJock()
    {
        Debug.Log("Jock");
        return;
        // Check to prevent useless power use
        if (pCatched._catchedRun)
        {
            pCatched._playerController.WinCatch();
        }
        else
        {
            hasPower = true;
        }
    }

    void UsePowerNerd()
    {
        AlarmeGeneral alarm = GameObject.Find("AlarmeManager").GetComponent<AlarmeGeneral>();

        Debug.Log("Nerd");

        // Check to prevent useless power use later
        if (alarm.alarmeAudio)
        {
            StartCoroutine(Hacking());
        }
        else
        {
            hasPower = true;
        }

    }

    public IEnumerator Hacking()
    {
        // BIP BIP BOOP BOOP
        Vector3 startPosition = transform.position;

        Debug.Log("Star Hacking");

        _SFXManager.ChangeState("Hacking");

        while (true)
        {
            /*if (transform.position != startPosition)
            {
                // Player moved
                Debug.Log("Hacking canceled");
                hackingProgression = 0.0f;
                hasPower = true;
                _SFXManager.ChangeState("Idle");
                break;
            }*/
            if (hackingProgression >= 2.0f)
            {
                Debug.Log("End Hacking");
                // Succes desactivating the alarm
                hackingProgression = 0.0f;
                AlarmeGeneral alarm = GameObject.Find("AlarmeManager").GetComponent<AlarmeGeneral>();
                Debug.Log(alarm);
                alarm.StopAlarmeCoroutine();
                alarm.StopAlarme();
                _SFXManager.ChangeState("Idle");
                break;
            }
            yield return new WaitForSeconds(0.10f);
            hackingProgression += Time.deltaTime;
        }
    }

    public void TryUsePower()
    {
        if (hasPower)
        {
            // Remove power
            hasPower = false;

            switch (_characterType)
            {
                case CharacterType.Artiste:
                    UsePowerArtiste();
                    break;
                case CharacterType.Influenceuse:
                    UsePowerInfluenceuse();
                    break;
                case CharacterType.Jock:
                    UsePowerJock();
                    break;
                case CharacterType.Nerd:
                    UsePowerNerd();
                    break;
                default:
                    break;
            }
        }
    }
}
