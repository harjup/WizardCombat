using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SkillPointStore
{
    public enum Skill
    {
        Acceleration,
        Power,
        Speed
    }

    private readonly Dictionary<Skill, int> _skillPointMap;

    //Firing an event when a function causes a level up. Good??? Bad???
    /*public delegate void LevelUp(Skill skill, int level);
    public event LevelUp OnLevelUp;
    protected virtual void OnOnLevelUp(Skill skill, int level)
    {
        LevelUp handler = OnLevelUp;
        if (handler != null) handler(skill, level);
    }*/


    public SkillPointStore()
    {
        _skillPointMap = new Dictionary<Skill, int>
        {
            {Skill.Acceleration, 0},
            {Skill.Power, 0},
            {Skill.Speed, 0}
        };

    }


    public void AddPoints(Skill skill, int amount)
    {
        _skillPointMap.Add(skill, amount);
    }

    public int GetLevel(Skill skill)
    {
        return _skillPointMap[skill] / 10;
    }

    public int GetPointTotal(Skill skill)
    {
        return _skillPointMap[skill];
    }

    public int PointsRequired(Skill skill)
    {
        return (_skillPointMap[skill] / 10) + 10;
    }

    public int GetPointsRemaining(Skill skill)
    {
        return _skillPointMap[skill] % 10;
    }
}
