using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pokemon_rand.src.main.model.utilities
{
    public class JsonUtilities
    {

        /// <summary>
        /// Generic method that deserializes a file into the given object
        /// </summary>
        /// 
        /// <param name="filename"> the string of the file's path </param>
        /// <typeparam name="T"> the object type to deserialize to </typeparam>
        /// 
        /// <returns> the object created after deserializing </returns>
        public async Task<T> JsonDeserializeAsync<T>(string filename) {

            var json = string.Empty;

            //open the file to read
            using (var fs = File.OpenRead(filename))

            //read the file
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync();

            T obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        /// <summary>
        /// Generic method that serializes an object into a json file
        /// </summary>
        /// 
        /// <param name="obj"> the obj to be serialized </param>
        /// <param name="filename"> the path of the file to be written to </param>
        /// <typeparam name="T"> the object type to serialize to </typeparam>
        public void JsonSerialize<T>(T obj, string filename) {
            
            //serialize the obj into a json formatted string
            string jsonStr = JsonConvert.SerializeObject(obj, formatting: Formatting.Indented);

            //write the json string into a file overwritting any previous data in it
            File.WriteAllText(filename, jsonStr);

        }

    }
}