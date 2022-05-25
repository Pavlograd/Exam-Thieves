using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.Networking;
using TMPro;

public class Matchmaker : MonoBehaviour
{
   [SerializeField] private TMP_Text textWait;
   [SerializeField] private ClientSetup _clientSetup;

   [SerializeField]private string QueueName;

   private Coroutine pollTicketCoroutine;
   private string ticketId;
   
   public void LeaveQueue()
   {
      PlayFabMultiplayerAPI.CancelMatchmakingTicket(
         new CancelMatchmakingTicketRequest
         {
            QueueName = QueueName,
            TicketId = ticketId,
         },
         OnTicketCanceled,
         OnMatchmakingError);
   }

   private void OnTicketCanceled(CancelMatchmakingTicketResult result)
   {
      StopCoroutine(pollTicketCoroutine);
   }

   public void Quit()
   {
      Application.Quit();
   }
   
   public void StartJoinQueu(bool solo)
   {
      textWait.text = "Waiting For Players...";
      if (solo)
      {
         NetworkManager.singleton.StartHost();
         return;
      }
      Debug.Log("Ticket creer");
      PlayFabMultiplayerAPI.CreateMatchmakingTicket(
         new CreateMatchmakingTicketRequest
         {
            Creator = new MatchmakingPlayer
            {
               Entity = new EntityKey
               {
                  Id = _clientSetup.EntityID,
                  Type = "title_player_account"
               },
               
               Attributes = new MatchmakingPlayerAttributes
               {
                  DataObject = new
                  {
                     Latency = new object[]
                     {
                        new {
                           region = "EastUs",
                           latency = 150
                        },
                        new {
                           region = "WestUs",
                           latency = 100
                        }
                     }
                  }
               }
            },
            GiveUpAfterSeconds = 120,
            
            QueueName = QueueName
         },
      OnMatchmakingTicketCreated,
      OnMatchmakingTicketError
      );
   }

   private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult result)
   {
      ticketId = result.TicketId;

      Debug.Log("Ticket creer ok");
      pollTicketCoroutine = StartCoroutine(PollTicket());
   }
   
   private void OnMatchmakingTicketError(PlayFabError error)
   {
      //Debug.Log(error.CustomData.ToString());
      Debug.LogError(error.GenerateErrorReport());
   }

   private IEnumerator PollTicket()
   {
      while (true)
      {
         PlayFabMultiplayerAPI.GetMatchmakingTicket(
            new GetMatchmakingTicketRequest
            {
               TicketId = ticketId,
               QueueName = QueueName,
            },
            OnGetMatchmakingTicket,
            OnMatchmakingError);

         yield return new WaitForSeconds(6);
      }
   }

   private void OnGetMatchmakingTicket(GetMatchmakingTicketResult result)
   {
      switch (result.Status)
      {
         case "Matched":
            StopCoroutine(pollTicketCoroutine);
            StartMatch(result.MatchId);
            break;
         
         case "Canceled":
            break;
      }
   }
   
   private void OnMatchmakingError(PlayFabError error)
   {
      Debug.LogError(error);  
   }

   private void StartMatch(string matchID)
   {
      textWait.text = "CONNECTING...";
      Debug.Log("Match trouver");
      PlayFabMultiplayerAPI.GetMatch(
         new GetMatchRequest
         {
            MatchId = matchID,
            QueueName = QueueName,
         },
         OnGetMatch,
         OnMatchmakingError);
   }

   private void OnGetMatch(GetMatchResult result)
   {
      //textWait.text = $"{result.Members[0].Entity.Id} vs {result.Members[1].Entity.Id}";
      Debug.Log(result.ToString());
      UnityNetworkServer.Instance.networkAddress = result.ServerDetails.IPV4Address;
      UnityNetworkServer.Instance.GetComponent<kcp2k.KcpTransport>().Port = (ushort)result.ServerDetails.Ports[0].Num;
        
      UnityNetworkServer.Instance.StartClient();
      NetworkClient.connection.Send<ReceiveAuthenticateMessage>(new ReceiveAuthenticateMessage()
      {
         PlayFabId = _clientSetup.playfabID
      });
   }
   
   
   
   
   
}
