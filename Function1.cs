using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Serilog;

public class Function1
{
    [FunctionName("Function1")]
    public void Run([QueueTrigger("message-queue", Connection = "MyQueueCon")] string myQueueItem, Microsoft.Extensions.Logging.ILogger log)


    {

     




    
    string Connstri = "Server=tcp:queuestorageserver1.database.windows.net,1433;Initial Catalog=dbQueues;Persist Security Info=False;User ID=kops;Password=Dbzgt1103;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

       
        
        
        
        
        try
        {
            string[] attributes = myQueueItem.Split(':');

           //INSERT VALUES HERE 
            string firstName = "Kopano";
            string lastName = "Fanana";
            string id = "02100151090";
            string center = "WATERFALL";
            string vaccinationDate = "2023/05/06";
            string serialNumber = "RRT7752";

            log.LogInformation($"Processing queue ID: {myQueueItem}");

            // Add the message to the 'message-queue'
            string queueName = "message-queue";
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=sakopano;AccountKey=0uJk9x/GShmzpqNjEigFSH5NXps5BMkAv13XTlreMmAV9IS/q9l97jHBU2ig7kPmdCm9OekazXYX+AStfC5QXw==;EndpointSuffix=core.windows.net");
            CloudQueueClient queueClient = cloudStorageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            string queueMessageContent = $"{firstName}:{lastName}:{id}:{center}:{vaccinationDate}:{serialNumber}";
            CloudQueueMessage queueMessage = new CloudQueueMessage(queueMessageContent);
            queue.AddMessageAsync(queueMessage);

            // Log successful message addition to the queue
            log.LogInformation($"Queue Message Added To the 'message-queue' successfully, Id = {id}");

            using (SqlConnection connection = new SqlConnection(Connstri))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    // SQL command to insert data into the 'messages_final' table
                    command.CommandText = "INSERT INTO VacTable (firstName, lastName, Id, CENTER, VaccinationDate, SerialNumber) VALUES (@firstName, @lastName, @Id, @Center, @VaccinationDate, @SerialNumber)";
                    command.Parameters.AddWithValue("@firstName", firstName);
                    command.Parameters.AddWithValue("@lastName", lastName);
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Center", center);
                    command.Parameters.AddWithValue("@VaccinationDate", vaccinationDate);
                    command.Parameters.AddWithValue("@SerialNumber", serialNumber);
                    command.ExecuteNonQuery();

                    // Add the entry to the 'VacTable'
                    CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
                    CloudTable VacTable = cloudTableClient.GetTableReference("VacTable");
                    VacTableEntity vaccinationEntry = new VacTableEntity(firstName, lastName, id, center, vaccinationDate, serialNumber);
                    TableOperation insertOp = TableOperation.Insert(vaccinationEntry);
                    VacTable.ExecuteAsync(insertOp);
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Error processing queue message: {ex.Message}");
        }
    }

    public class VacTableEntity : TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Id { get; set; }
        public string Center { get; set; }
        public string VaccinationDate { get; set; }
        public string SerialNumber { get; set; }

        public VacTableEntity(string firstName, string lastName, string id, string center, string vaccinationDate, string serialNumber)
        {
            this.PartitionKey = id;
            this.RowKey = center;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Id = id;
            this.Center = center;
            this.VaccinationDate = vaccinationDate;
            this.SerialNumber = serialNumber;
        }
    }

}
