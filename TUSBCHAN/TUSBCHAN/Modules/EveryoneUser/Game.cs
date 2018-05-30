using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUSBCHAN.Modules.EveryoneUser
{
    public class Game : ModuleBase
    {
        [Command("coin")]
        [Summary("コイントスを行いおもてかうらを決めます。")]
        public async Task Coin([Summary("コイントス回数")]int count = 1)
        {
            var sb = new StringBuilder();
            var rnd = new System.Random();
            int omote = 0;
            if (count == 0)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                if (rnd.Next(0, 2) == 0)
                {
                    sb.Append("おもて ");
                    omote++;
                }
                else
                {
                    sb.Append("うら ");
                }

            }

            await ReplyAsync(string.Format("結果:{0}\n表:{1}回\n裏:{2}回", sb.ToString(), omote, count - omote));
        }

        [Command("quiz")]
        [Summary("クイズを出題します")]
        public async Task Quiz()
        {
            var list = new API.Quiz.Quiz().GetQuiz();
            var answersb = new StringBuilder();
            var answers = new string[4];

            answers[0] = list.answer1;
            answers[1] = list.answer2;
            answers[2] = list.answer3;
            answers[3] = list.answer4;
            answers = answers.OrderBy(i => Guid.NewGuid()).ToArray();
            var dictionary = new Dictionary<string, string>();
            dictionary.Add(answers[0], "💙");
            dictionary.Add(answers[1], "💚");
            dictionary.Add(answers[2], "💛");
            dictionary.Add(answers[3], "💜");
            foreach (var element in dictionary)
            {
                answersb.Append($"{element.Value} {element.Key}\n");
            }

            var message = await ReplyAsync($"問題：{list.question}\n残り30秒\n{answersb.ToString()}");
            await message.AddReactionAsync(new Emoji(dictionary[answers[0]]));
            await message.AddReactionAsync(new Emoji(dictionary[answers[1]]));
            await message.AddReactionAsync(new Emoji(dictionary[answers[2]]));
            await message.AddReactionAsync(new Emoji(dictionary[answers[3]]));
            for (int time = 30; time > 0; time = time - 10)
            {
                await message.ModifyAsync(a => a.Content = $"問題：{list.question}\n残り{time}秒\n{answersb.ToString()}");
                await Task.Delay(10000);
            }
            await message.ModifyAsync(a => a.Content = $"問題：{list.question}\n回答受付終了\n{answersb.ToString()}");
            await ReplyAsync($"正解は{list.answer1}でした");

            var userdictionary = new Dictionary<string, int>();

            foreach (var element in dictionary)
            {
                var users = await message.GetReactionUsersAsync(element.Value);
                foreach (var user in users)
                {
                    if (!user.IsBot)
                    {
                        if (!userdictionary.ContainsKey(user.Username))
                        {
                            userdictionary.Add(user.Username, 0);
                        }
                        if (dictionary[list.answer1] == element.Value)
                        {
                            userdictionary[user.Username]++;
                        }
                        else
                        {
                            userdictionary[user.Username]--;
                        }
                        
                    }
                }
            }

            if (userdictionary.Count > 0)
            {
                var corrects = new StringBuilder();
                foreach(var element in userdictionary)
                {
                    if (element.Value == 1)
                    {
                        corrects.Append(element.Key + "、");
                    }
                }
                if (corrects.Length > 0)
                {
                    await ReplyAsync($"{ corrects.ToString().TrimEnd('、')}さんが正解したよ！賢い人は私好きですよ");
                }
                else
                {
                    await ReplyAsync($"みんなハズレだ自分の学のなさを反省しろ");
                    
                }
            }
            else
            {
                await ReplyAsync("誰も回答してくれませんでした...");
            }
        }
    }
}
