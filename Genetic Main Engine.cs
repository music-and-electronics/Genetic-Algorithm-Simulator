using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace Genetic_AI
{
    class Genetic_Main_Engine
    {
        public string raw_json_data { get; set; }
        public JObject genetic_raw_object { get; set; }

        public List<int[]> gene_set;
        public List<int[]> survived_genes;

        Random random_generator = new Random();

        public int[] genetic_target_data { get; set; }
        public int generation;

        public void Json_parser()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            Nullable<bool> result = openFileDialog.ShowDialog();
            raw_json_data = File.ReadAllText(openFileDialog.FileName);
            genetic_raw_object = JObject.Parse(raw_json_data);

        }

        public void Gene_Initialize()
        {
            generation = 0;
            gene_set = new List<int[]>();

            genetic_target_data = new int[(int)genetic_raw_object["Genetic Length"]];

            Gene_generator();

            for (int i = 0; i < (int)genetic_raw_object["Genetic Length"]; i++)
            {
                genetic_target_data[i] = random_generator.Next((int)genetic_raw_object["Genetic Range"][0], (int)genetic_raw_object["Genetic Range"][1]);
            }

        }

       public  void Gene_generator()
        {
            for (int i = 0; i < (int)genetic_raw_object["Entity"]; i++)
            {
                int[] genetic_member = new int[(int)genetic_raw_object["Genetic Length"]];

                for (int k = 0; k < genetic_member.Length; k++)
                {
                    genetic_member[k] = random_generator.Next((int)genetic_raw_object["Genetic Range"][0], (int)genetic_raw_object["Genetic Range"][1]);
                }
                gene_set.Add(genetic_member);
            }
        }

       public void Gene_generator(List<int[]> survived_genes)
        {

            for (int i = 0; i < 2; i++)
            {
                for (int k = 0; k < survived_genes[0].Length; k++)
                {
                    gene_set[i][k] = survived_genes[i][k];
                }
            }

            for (int i = 2; i < gene_set.Count - 1; i++)
            {
                int genetic_mix_rand_num = random_generator.Next(survived_genes[0].Length);
                double mutant_probabilty = (double)genetic_raw_object["Mutant Probability"] * 100;

                if (random_generator.Next(1,100)<=mutant_probabilty)
                {
                    for (int k = 0; k < genetic_mix_rand_num; k++)
                    {
                        gene_set[i][k] = survived_genes[0][k];
                    }

                    for (int k = genetic_mix_rand_num; k < survived_genes[0].Length; k++)
                    {
                        gene_set[i][k] = survived_genes[1][k];
                    }

                    gene_set[i][genetic_mix_rand_num] = random_generator.Next((int)genetic_raw_object["Genetic Range"][0], (int)genetic_raw_object["Genetic Range"][1]);
                }
                else if(random_generator.Next(1,100) <100 && random_generator.Next(1,100) >=50)
                {
                    for (int k = 0; k < genetic_mix_rand_num / 2; k++)
                    {
                        gene_set[i][genetic_mix_rand_num / 2+ k] = survived_genes[0][k];
                    }

                    for (int k = genetic_mix_rand_num / 2; k < genetic_mix_rand_num; k++)
                    {
                        gene_set[i][k- genetic_mix_rand_num / 2] = survived_genes[1][k];
                    }
                }
                else
                {
                    for (int k = 0; k < genetic_mix_rand_num / 2; k++)
                    {
                        int odd = 2 * k + 1;
                        gene_set[i][odd] = survived_genes[0][k];
                    }

                    for (int k = 1; k < genetic_mix_rand_num / 2; k++)
                    {
                        int even = 2 * k;
                        gene_set[i][even] = survived_genes[1][k];
                    }
                }
            }

            survived_genes.RemoveAt(0);
            survived_genes.RemoveAt(0);
        }

        public void Survive(List<int[]> gene_set, int[] genetic_target_data)
        {

            survived_genes = new List<int[]>();

            List<Score_Data> Score = new List<Score_Data>();

            foreach (int[] gene in gene_set)
            {
                Score.Add(new Score_Data(Score_Calculation(genetic_target_data, gene), gene));
            }

            Score.Sort(
                delegate (Score_Data A, Score_Data B)
                {
                    if (A.score > B.score) return 1;
                    else if (A.score == B.score) return 0;
                    else return -1;
                }
            );

            survived_genes.Add((Score[0].gene_data));
            survived_genes.Add((Score[1].gene_data));
            generation++;

        }

        public int Score_Calculation(int[] goal_gene, int[] operand_gene)
        {
            int temp_sum = 0;
            
            for (int i = 0; i < goal_gene.Length; i++)
            {
                temp_sum += (goal_gene[i] - operand_gene[i]) * (goal_gene[i] - operand_gene[i]);
            }
            return temp_sum;
        }

        public bool total_Score_Caculation(int[] goal_gene,List<int[]>operand_gene)
        {
            int temp_sum = 0;
            foreach(int [] gene in operand_gene)
            {
                temp_sum+=Score_Calculation(goal_gene, gene);
            }
            if (temp_sum == 0)
                return true;
            else
                return false;
        }
    }

    public class Score_Data
    {
        public int score { get; set; }
        public int[] gene_data { get; set; }

        public Score_Data(int tmp_score, int[] tmp_gene_data)
        {
            gene_data = tmp_gene_data;
            score = tmp_score;
        }
    }
}