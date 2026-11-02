using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    private List<Portal> portals;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // once generation is complete, find and connect all portals
        MapGenerator mapGenerator = FindAnyObjectByType<MapGenerator>();
        if (mapGenerator)
        {
            // kind of a corruption of OnPlayerPlaced, we don't really care about the player but it seems a waste to create a whole new event
            mapGenerator.OnPlayerPlaced += ConnectPortals;
        } else
        {
            ConnectPortals(null);
        }
    }

    private void ConnectPortals(Player _player)
    {
        // find all portals
        portals = new List<Portal>(FindObjectsByType<Portal>(FindObjectsSortMode.None));
        // Assign each portal a random partner
        System.Random randy = new();
        while (portals.Count > 1) {
            int randIdx1 = randy.Next(portals.Count);
            Portal portal1 = portals[randIdx1];
            portals.Remove(portal1);
            int randIdx2 = randy.Next(portals.Count);
            Portal portal2 = portals[randIdx2];
            portals.Remove(portal2);
            portal1.pair = portal2;
            portal1.Activate();
            portal2.pair = portal1;
            portal2.Activate();
        }
        // we only want an even number of portals
        if (portals.Count == 1)
        {
            Destroy(portals[0]);
        }
    }


    public void AddPortal(Portal portal)
    {
        portals.Add(portal);
    }
}
