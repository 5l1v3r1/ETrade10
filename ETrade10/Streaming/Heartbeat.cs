﻿namespace OkonkwoETrade10.REST.Streaming
{
   public class Heartbeat : IHeartbeat
   {
      /// <summary>  
      /// The string “HEARTBEAT”
      /// </summary>
      public string type { get; set; }

      /// <summary>
      /// The date/time when the TransactionHeartbeat was created.
      /// </summary>
      public string time { get; set; }
   }

   public interface IHeartbeat
   {
      /// <summary>  
      /// The string “HEARTBEAT”
      /// </summary>
      string type { get; set; }

      /// <summary>
      /// The date/time when the TransactionHeartbeat was created.
      /// </summary>
      string time { get; set; }
   }
}
