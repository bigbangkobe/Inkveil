using System;
using System.Collections.Generic;
[Serializable]
public class StageRewardsInfo
{
    public int stageID;
    public string stageName;
    public int firstClearExp;
    public string rewards;
    public string specialAwards;
    public bool isPass;
    public int grade;
    public int[] receive = new int[3];

    private int[][] rewardsArry;
    public int[][] GetRewards()
    {
        if (rewardsArry == null)
        {
            string[] rows = rewards.Split('|');
            int[][] twoDArray = new int[rows.Length][];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                twoDArray[i] = Array.ConvertAll(elements, int.Parse);
            }
            rewardsArry = twoDArray;
        }
        return rewardsArry;
    }

    private int[][] specialAwardssArry;
    public int[][] GetSpecialAwards()
    {
        if (specialAwardssArry == null)
        {
            string[] rows = specialAwards.Split('|');
            int[][] twoDArray = new int[rows.Length][];

            for (int i = 0; i < rows.Length; i++)
            {
                string[] elements = rows[i].Split(',');
                twoDArray[i] = Array.ConvertAll(elements, int.Parse);
            }
            specialAwardssArry = twoDArray;
        }
        return specialAwardssArry;
    }


    private List<PropertyInfo> specialAwardsPropertyInfos;
    public List<PropertyInfo> GetSpecialAwardsPropertyInfos()
    {
        List<PropertyInfo> infos = new List<PropertyInfo>();
        if (specialAwardsPropertyInfos == null)
        {
           
            int[][] res = GetSpecialAwards();

            for (int i = 0; i < res.Length; i++)
            {
                PropertyInfo propertyInfo = PropertyDispositionManager.instance.GetPropertyById(res[i][0]);
                propertyInfo.number = res[i][1];
                infos.Add(propertyInfo);
            }
            //specialAwardsPropertyInfos = infos;
        }
        return infos;
    }


    private List<PropertyInfo> rewardsPropertyInfos;
    public List<PropertyInfo> GetRewardsPropertyInfos()
    {
        List<PropertyInfo> infos = new List<PropertyInfo>();
        if (rewardsPropertyInfos == null)
        {
           
            int[][] res = GetRewards();

            for (int i = 0; i < res.Length; i++)
            {
                PropertyInfo propertyInfo = PropertyDispositionManager.instance.GetPropertyById(res[i][0]);
                propertyInfo.number = res[i][1];
                infos.Add(propertyInfo);
            }
            //rewardsPropertyInfos = infos;
        }
        return infos;
    }
}