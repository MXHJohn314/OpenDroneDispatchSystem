namespace FrontEnd.Shared
{
    public class QueryGen
    {
        public static string connectionString = null;

        public static string Generate(string collName, string arrayName = "", string filterType = "", string filterVal = "", string sort = "")
        {

            if (arrayName == "" && filterType == "" && filterVal == "" && sort == "")
            {
                connectionString = "http://localhost:8080/" + collName;
                return connectionString;
            }
            else if(sort != "" && arrayName == "" && filterType == "" && filterVal == "")
            {
                connectionString = "http://localhost:8080/" + collName + "?sort={" + sort + "}";
                return connectionString;
            }
            else if (filterType != "" && filterVal != "" && sort == "" && arrayName == "")
            {
                connectionString = "http://localhost:8080/" + collName + "?filter={'" + filterType + "':'" + filterVal + "'}";
                return connectionString;
            }
            else if (filterType != "" && filterVal != "" && sort != "" && arrayName == "")
            {
                connectionString = "http://localhost:8080/" + collName + "?filter={'" + filterType + "':'" + filterVal + "'}&sort={" + sort + "}";
            }
            else if (filterType != "" && filterVal != "" && arrayName != "" && sort == "")
            {
                connectionString = "http://localhost:8080/" + collName + "?filter={'" + arrayName + "':{'" + filterType + "':'" + filterVal + "'}";
            }
            return connectionString;


        }
    }
}