using System;

public class LevelInfo
{
    public int levelID;
    public string levelGrade;
    public double firstTime;
    public string enemyGroup;
    public string sceneName;
    public string skillGroup;

    private int[][] enemyGroupArry;
    private int[][] skillGroupArry;
    public int[][] GetEnemyGroup()
    {
        if (enemyGroupArry == null)
        {
            string[] rows = enemyGroup.Split('|');
            int[][] twoDArray = new int[rows.Length][];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                twoDArray[i] = Array.ConvertAll(elements, int.Parse);
            }
            enemyGroupArry = twoDArray;
        }
        return enemyGroupArry;
    }

    public int[][] GetSkillGroup()
    {
        if (skillGroupArry == null)
        {
            string[] rows = skillGroup.Split('|');
            int[][] twoDArray = new int[rows.Length][];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                twoDArray[i] = Array.ConvertAll(elements, int.Parse);
            }
            skillGroupArry = twoDArray;
        }
        return skillGroupArry;
    }
}