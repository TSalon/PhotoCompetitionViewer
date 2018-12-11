using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPhotoCompetitionViewer.Competitions
{
    class HoldTools
    {
        internal static bool ToggleHeld(SQLiteConnection dbConnection, String imagePath)
        {
            bool isHeld = HoldTools.IsHeld(dbConnection, imagePath);

            dbConnection.Open();

            if (isHeld == false)
            {
                // Not held - hold it in the database
                String sql = "INSERT INTO held_images (timestamp, name) VALUES (CURRENT_TIMESTAMP, @name)";

                SQLiteCommand insertHeld = new SQLiteCommand(sql, dbConnection);
                insertHeld.Parameters.Add(new SQLiteParameter("@name", imagePath));
                insertHeld.ExecuteNonQuery();
            }
            else
            {
                // Already held - remove it from the database
                String sql = "DELETE FROM held_images WHERE name = @name";
                SQLiteCommand deleteHeld = new SQLiteCommand(sql, dbConnection);
                deleteHeld.Parameters.Add(new SQLiteParameter("@name", imagePath));
                deleteHeld.ExecuteNonQuery();
            }

            dbConnection.Close();
            return !isHeld;
        }

        internal static bool IsHeld(SQLiteConnection dbConnection, string imagePath)
        {
            dbConnection.Open();

            bool isHeld = false;
            string isHeldSql = "SELECT COUNT(*) FROM held_images WHERE name = @name";

            SQLiteCommand cmd = new SQLiteCommand(isHeldSql, dbConnection);
            cmd.Parameters.Add(new SQLiteParameter("@name", imagePath));
            SQLiteDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isHeld = reader.GetInt16(0) > 0;
                }
            }
            reader.Close();


            dbConnection.Close();

            return isHeld;
        }
    }
}
