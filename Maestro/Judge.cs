﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Maestro
{
    public class Judge
    {
        private List<Step> _stepList { get; set; }

        public Difficulty _selectedDifficulty { get; set; }

        #region constants
        const int ERRORMARGIN = 1001;
        const int BADMARGIN = 600;
        const int GOODMARGIN = 400;
        const int EXCELLENTMARGIN = 200;

        const int BADMARK = 10;
        const int GOODMARK = 20;
        const int EXCELLENTMARK = 30;
        #endregion

        //Last judged note
        private int _lastIndex;
        public static int combo;

        public Judge()
        {
            _lastIndex = 0;
            combo = 1;
        }

        public void updateSteps(List<Step> listOfSteps)
        {
            _stepList = listOfSteps;
        }

        //Difficulty selection
        public void selectDifficulty(Difficulty dif)
        {
            _selectedDifficulty = dif;
        }

        //Returns the score of the current frame
        public int getScore(int lHand, int rHand, int lFoot, int rFoot, int currentTime, Point lHandP, Point rHandP, int[] scoreTable)
        {
            Step currentStep;

            int score = 0;
            
            //Foreach step
            for (int i = _lastIndex; i < _stepList.Count; i++)
            {
                currentStep = _stepList.ElementAt(i);

                //Check for the hold first
                if (currentStep.action == ActionType.HoldFoot || currentStep.action == ActionType.HoldHand)
                {
                    //If timing is ok
                    if (currentTime <= (currentStep.timing + currentStep.holdTime) && currentTime>=currentStep.timing)
                    {
                        switch (currentStep.action)
                        {
                            case ActionType.HoldHand:

                                if ((currentStep.area == rHand || currentStep.area == lHand))
                                {
                                    Console.WriteLine("left hand");
                                    score += 10;
                                }
                                break;

                            case ActionType.HoldFoot:
                                if ((currentStep.area == lFoot || currentStep.area == rFoot))
                                {
                                    Console.WriteLine("left hand");
                                    score += 10;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }else if (currentTime > currentStep.timing + ERRORMARGIN)
                {
                   // return score;
                    if (!currentStep.done)
                        combo = 1;

                }                
                //Clap case
                else if (!currentStep.done && currentStep.action == ActionType.Clap && currentStep.stepDifficulty == _selectedDifficulty && currentTime - ERRORMARGIN < currentStep.timing && currentStep.timing < currentTime + ERRORMARGIN)
                {

                    //Check the distance between the hands
                    if (Math.Sqrt(Math.Pow((lHandP.X - rHandP.X), 2) + Math.Pow((lHandP.Y - rHandP.Y), 2)) < 60 && rHand == currentStep.area)
                    {
                        score = evaluate(currentTime, currentStep, score, scoreTable);
                        _lastIndex = i;

                    }

                }
                //Touch case
                else if (!currentStep.done && currentStep.stepDifficulty == _selectedDifficulty && currentTime - ERRORMARGIN < currentStep.timing && currentStep.timing < currentTime + ERRORMARGIN)
                {
                    //If touch left hand
                    if (currentStep.action == ActionType.TouchHandLeft && (lHand == currentStep.area))
                    {
                        //Current step is done
                        _lastIndex = i;
                        score = evaluate(currentTime, currentStep, score, scoreTable);
                    }
                    //If touch left foot
                    else if (currentStep.action == ActionType.TouchFeetLeft && (lFoot == currentStep.area))
                    {
                        //Current step is done
                        _lastIndex = i;
                        score = evaluate(currentTime, currentStep, score, scoreTable);
                    }
                    //If touch right hand
                    else if (currentStep.action == ActionType.TouchHandRight && rHand == currentStep.area)
                    {
                        //Current step is done
                        _lastIndex = i;
                        score = evaluate(currentTime, currentStep, score, scoreTable);
                    }
                    else if (currentStep.action == ActionType.TouchFeetRight && rFoot == currentStep.area)
                    {

                        //Current step is done
                        _lastIndex = i;
                        
                        score = evaluate(currentTime, currentStep, score, scoreTable);
                    }
                }
            }
            return score;
        }

        private static int evaluate(int currentTime, Step currentStep, int frameScore, int[] scoreTable)
        {
            //Check the timing
            if (currentStep.timing - BADMARGIN < currentTime && currentTime < currentStep.timing + BADMARGIN)
            {
                if (currentStep.timing - GOODMARGIN < currentTime && currentTime < currentStep.timing + GOODMARGIN)
                {
                    if (currentStep.timing - EXCELLENTMARGIN < currentTime && currentTime < currentStep.timing + EXCELLENTMARGIN)
                    {
                        frameScore += EXCELLENTMARK;
                        scoreTable[0]++;
                        currentStep.step_Done(Score.Excellent);
                        combo++;
                    }
                    //GOOD MARK
                    else
                    {
                        frameScore += GOODMARK;
                        scoreTable[1]++;
                        currentStep.step_Done(Score.Good);
                        combo++;
                    }
                }
                //BAD MARK
                else
                {
                    frameScore += BADMARK;
                    scoreTable[2]++;
                    currentStep.step_Done(Score.Bad);
                    combo = 1;
                }
            }
            return frameScore;
        }
    }
}
