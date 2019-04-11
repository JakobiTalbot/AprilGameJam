using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour {

    public GameObject[] listOfTiles = new GameObject[9];        // List of tiles in play area
    public GameObject[] listOfEvents = new GameObject[6];       // List of events that can occur

    public GameObject nextEventTile;                            // Next tile an event will occur on
    public GameObject previousEventTile;                        // Tile event has just occured on

    public Vector3 tilePosition;                                // Position of tile next event

    public GameObject incomingEvent;                            // Next event to occur
    public GameObject previousEvent;                            // Event that has just occured

    [Range(5.0f, 30.0f)]
    public float eventInterval = 10f;                           // Time between events being chosen
    private float eventTimer = 0f;

    public float eventCountdown = 5.0f;                         // Once event is chosen start countdown until event happens
	
	// Update is called once per frame
	void Update () {
        eventTimer += Time.deltaTime;

        if (eventTimer >= eventInterval)
        {
            eventCountdown -= Time.deltaTime;

            if(nextEventTile == null)
            {
                NextTileSelection();
                tilePosition = nextEventTile.transform.position;
            }

            if(incomingEvent == null)
                NextEventSelection();

            if(eventCountdown <= 0)
            {
                Instantiate(incomingEvent, tilePosition, Quaternion.identity);

                previousEventTile = nextEventTile;
                nextEventTile = null;

                previousEvent = incomingEvent;
                incomingEvent = null;

                eventTimer = 0f;
                eventCountdown = 5f;
            }
        }
	}

    // Selects next tile to play event from
    void NextTileSelection()
    {
        int tileNumber;

        tileNumber = Random.Range(0, listOfTiles.Length);
        nextEventTile = listOfTiles[tileNumber];

        if(previousEventTile == nextEventTile)
        {
            NextTileSelection();
        }
    }

    // Selects next event to take place
    void NextEventSelection()
    {
        int eventNumber;

        eventNumber = Random.Range(0, listOfEvents.Length);
        incomingEvent = listOfEvents[eventNumber];

        if(previousEvent == incomingEvent)
        {
            NextEventSelection();
        }
    }
}
