using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using pokemon_rand.src.main.model.utilities;

namespace pokemon_rand.src.main.model.persistence
{
    public class ObjectFileDAO<T>
    {
        protected Dictionary<ulong, T> data;
        protected string jsonString;
        protected JsonUtilities jsonUtilities;

        /// <summary>
        /// Constructor for the PlayersFileDAO object
        /// setups the DAO object
        /// loads the player datas
        /// </summary>
        /// <param name="playersJson"> The path to the json file </param>
        /// <param name="jsonUtilities"> An object that helps with json manipulations </param>
        public ObjectFileDAO(string jsonString, JsonUtilities jsonUtilities)
        {

            this.jsonString = jsonString;
            this.jsonUtilities = jsonUtilities;
            this.data = new Dictionary<ulong, T>();
            load();
        }

        /// <summary>
        /// Deserializes the players json file into a collection of players and their data
        /// The data is then saved into a local collection for easier access
        /// </summary>
        private void load()
        {
            var tempData = jsonUtilities.JsonDeserializeAsync<Dictionary<ulong, T>>(jsonString).Result;
            if (tempData != null)
            {
                this.data = tempData;
            }
        }

        /// <summary>
        /// Serializes the local collection of player data into the json file
        /// This acts as saving player data
        /// </summary>
        public void save()
        {
            jsonUtilities.JsonSerialize(data, jsonString);
        }

        /// <summary>
        /// Retrieves a specific player based on their UID
        /// </summary>
        /// <param name="UID"> The UID of the player to be receieved </param>
        /// /// <returns> The player associated with the UID, null if not found </returns>
        public T getObject(ulong id)
        {
            T result;
            data.TryGetValue(id, out result);
            return result;
        }

        /// <summary>
        /// Gets a collection of all players currently in the database
        /// </summary>
        /// <returns> An array of all players that are saved in the database </returns>
        public T[] getObjects()
        {
            return data.Values.ToArray();
        }

        /// <summary>
        /// Adds a new player
        /// Creates a new player and adds them to the local collection then saves
        /// </summary>
        /// <param name="member"> The discord member to add </param>
        /// <returns> A boolean to indicate whether or not the player was successfully created </returns>
        public void addObject(T obj, ulong id)
        {
            this.data[id] = obj;
            save();
        }

        /// <summary>
        /// Deletes a player from the database, erasing their data
        /// </summary>
        /// <param name="UID"> The UID of the player to delete </param>
        /// <returns> Returns a boolean indicating whether or not the deletion was successful </returns>
        public bool deleteObject(ulong id)
        {
            data.Remove(id);
            save();
            return true;
        }

        public void deleteAll() {
            data.Clear();
            save();
        }

        /// <summary>
        /// Retrieves the requested player's status information
        /// </summary>
        /// <param name="UID"> the id of the player to search for </param>
        /// <returns> the player's information in a string </returns>
        public string getString(ulong UID)
        {
            T currObj = this.getObject(UID);
            return currObj.ToString();
        }
    }
}