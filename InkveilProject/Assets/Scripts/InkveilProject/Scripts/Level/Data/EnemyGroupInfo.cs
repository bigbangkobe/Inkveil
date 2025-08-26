using System;
[Serializable]
public class EnemyGroupInfo
{
    public int enemyGroupID;
    public string enemyGroupInformation;
    public int AppearanceMode;
    public double AppearanceContent;

    private int[][] enemyArry;
    public int[][] GetEnemyGroup()
    {
        if (enemyArry == null)
        {
            string[] rows = enemyGroupInformation.Split('|');
            int[][] twoDArray = new int[rows.Length][];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                twoDArray[i] = Array.ConvertAll(elements, int.Parse);
            }
            enemyArry = twoDArray;
        }
        return enemyArry;
    }
}