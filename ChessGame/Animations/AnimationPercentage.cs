using ChessGame.GameObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGame.Animations
{
    public class AnimationPercentage : Animation
    {
        Dictionary<int, float> keys = new Dictionary<int, float>();
        Dictionary<int, KeyFrame> posKeys = new Dictionary<int, KeyFrame>();
        Vector3 destinationPos;
        Vector3 lastPos;
        Vector3 currentDestPos;

        public AnimationPercentage(ChessGame game, Vector3 originPos, Vector3 destinationPos, Dictionary<int, float> keys) : base(game)
        {
            this.keys = keys;
            this.destinationPos = destinationPos;
            lastPos = originPos;
            currentDestPos = originPos;
            float dx = destinationPos.X - originPos.X;
            float dy = destinationPos.Y - originPos.Y;
            float dz = destinationPos.Z - originPos.Z;
            int j = 0;
            foreach (int i in keys.Keys)
            {
                KeyFrame key = new KeyFrame(j, i, new Vector3(originPos.X + dx * keys[i], originPos.Y + dy * keys[i], originPos.Z + dz * keys[i]));
                posKeys.Add(j, key);
                j++;
            }
            enabled = true;
        }

        int elapsedTime = 0;
        int j = 0;
        public void update(GameObject obj)
        {
            if (!enabled)
            {
                return;
            }

            if (posKeys.ContainsKey(j))
            {
                int neededTime = posKeys[j].time;
                if(neededTime == elapsedTime)
                {
                    lastPos = currentDestPos;
                    currentDestPos = posKeys[j].pos;
                    j++;
                }
            }
            elapsedTime += 1;

            float dx = currentDestPos.X - lastPos.X;
            float dy = currentDestPos.Y - lastPos.Y;
            float dz = currentDestPos.Z - lastPos.Z;
            if(posKeys.Count > j + 1)
            {
                Vector3 pos = new Vector3(lastPos.X + dx / posKeys[j + 1].time, lastPos.Y + dy / posKeys[j + 1].time, lastPos.Z + dz / posKeys[j + 1].time);
                obj.moveTo(pos);
            }
        }


    }
}
