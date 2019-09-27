using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading.Tasks;
namespace Genetic_AI
{
    public partial class MainWindow : Window
    {
        Genetic_Main_Engine genetic_Main_Engine = new Genetic_Main_Engine();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Json_Button_Click(object sender, RoutedEventArgs e)
        {
            textbox_Json_Entity.Clear();
            textbox_Json_Genetic_Range.Clear();
            textbox_Json_Gene_Length.Clear();
            textbox_Json_Mutant_Prob.Clear();
            genetic_Main_Engine.Json_parser();
            textbox_Json_Entity.AppendText((string)genetic_Main_Engine.genetic_raw_object["Entity"]);
            textbox_Json_Genetic_Range.AppendText
                (
                (string)genetic_Main_Engine.genetic_raw_object["Genetic Range"][0]
                + "~" +
                (string)genetic_Main_Engine.genetic_raw_object["Genetic Range"][1]
                );
            textbox_Json_Gene_Length.AppendText((string)genetic_Main_Engine.genetic_raw_object["Genetic Length"]);
            textbox_Json_Mutant_Prob.AppendText(((double)genetic_Main_Engine.genetic_raw_object["Mutant Probability"] * 100).ToString() + "%");
        }

        private void Initialize_Gene_Click(object sender, RoutedEventArgs e)
        {
            Canvas1.Children.Clear();
            genetic_Main_Engine.Gene_Initialize();
            int index = 0;

            textblock_TargetData.Text = $"Target Data\nGeneration : {genetic_Main_Engine.generation.ToString()}";
            textblock_Data.Text = "Data";
            for (int i = 0; i < genetic_Main_Engine.genetic_target_data.Length; i++)
            {

                Canvas1.Children.Add(Create_Rectangle(i, Canvas1.Margin.Bottom - 140, genetic_Main_Engine.genetic_target_data[i], 1));

            }

            foreach (int[] gene in genetic_Main_Engine.gene_set)
            {
                for (int i = 0; i < gene.Length; i++)
                {
                    Canvas1.Children.Add(Create_Rectangle(i, index, gene[i], 0));
                }
                index++;
            }

        }

        Rectangle Create_Rectangle(double desiredCenterX, double desiredCenterY, int color, int target)
        {
            int bytecolor ;
            double length_ratio =  1 /  (double)genetic_Main_Engine.genetic_raw_object["Genetic Length"] ;
            double depth_ratio= 1/  ((double)genetic_Main_Engine.genetic_raw_object["Entity"]*1.2);
            double betweensize;
            double vertical_gap;
        
            if (target == 0)
            {
                bytecolor = (color*1515) % 255;
                betweensize = desiredCenterX * 1000 * length_ratio;
                vertical_gap = desiredCenterY * 1500 * depth_ratio + Canvas1.Margin.Bottom;
            }

            else
            {
                bytecolor = (color*1515) % 255;
                betweensize = desiredCenterX * 1000 * length_ratio;
                vertical_gap = desiredCenterY-150;
            }

            Rectangle rectangle = new Rectangle { Width = 600 * length_ratio, Height = 600 * depth_ratio, Fill = new SolidColorBrush(Color.FromRgb((byte)bytecolor, 100 ,100)) };
            double left = betweensize - (5 / 2);
            double bottom = -vertical_gap - 500;
            rectangle.Margin = new Thickness(10+ left, 0, 0, bottom);
            return rectangle;
        }

        async private void Goto_Next_Generation_Click(object sender, RoutedEventArgs e)
        {

            while (genetic_Main_Engine.Score_Calculation(genetic_Main_Engine.genetic_target_data, genetic_Main_Engine.gene_set[0]) >0)
            {
                genetic_Main_Engine.Survive(genetic_Main_Engine.gene_set, genetic_Main_Engine.genetic_target_data);
                genetic_Main_Engine.Gene_generator(genetic_Main_Engine.survived_genes);
                int entity = (int)genetic_Main_Engine.genetic_raw_object["Entity"];
                int index = 0;
                textbox_GeneList.Clear();
                await Task.Run(() =>
                {
                    Canvas1.Dispatcher.Invoke(()
                         => Canvas1.Children.Clear(), System.Windows.Threading.DispatcherPriority.Background);
                    
                    Canvas1.Dispatcher.Invoke(()
                      => textblock_TargetData.Text= $"Target Data\nGeneration : {genetic_Main_Engine.generation.ToString()}", System.Windows.Threading.DispatcherPriority.Background);

                    Canvas1.Dispatcher.Invoke(()
                          => textblock_Data.Text= "Data", System.Windows.Threading.DispatcherPriority.Background);
                    Canvas1.Dispatcher.Invoke(()
                    =>
                    {
                        for (int i = 0; i < genetic_Main_Engine.genetic_target_data.Length; i++)
                        {
                            Canvas1.Children.Add(Create_Rectangle(i, Canvas1.Margin.Bottom - 140, genetic_Main_Engine.genetic_target_data[i], 1));
                            
                        }
                    }
                    , System.Windows.Threading.DispatcherPriority.Background);
                    Canvas1.Dispatcher.Invoke(()
                     =>
                    {
                        foreach (int[] gene in genetic_Main_Engine.gene_set)
                        {
                            int score = genetic_Main_Engine.Score_Calculation(genetic_Main_Engine.genetic_target_data, gene);
                            Canvas1.Dispatcher.Invoke(()
                     =>
                            {
                                for (int i = 0; i < gene.Length; i++)
                                {
                                    Rectangle rec = Create_Rectangle(i, index, gene[i], 0);
                                    Canvas1.Children.Add(rec);
                                }
                                textbox_GeneList.AppendText($"index{index} Score: " + genetic_Main_Engine.Score_Calculation(gene, genetic_Main_Engine.genetic_target_data).ToString() + "\n");

                            }, System.Windows.Threading.DispatcherPriority.Background
                        );
                            index++;
                        }
                    }, System.Windows.Threading.DispatcherPriority.Background
                        );
                }
              );
            }
            MessageBox.Show("Calculation Completed!");
        }

        private void Goto_Manual_Next_Generation_Click(object sender, RoutedEventArgs e)
        {
            genetic_Main_Engine.Survive(genetic_Main_Engine.gene_set, genetic_Main_Engine.genetic_target_data);
            genetic_Main_Engine.Gene_generator(genetic_Main_Engine.survived_genes);
            int entity = (int)genetic_Main_Engine.genetic_raw_object["Entity"];
            int index = 0;

            Canvas1.Children.Clear();
            textbox_GeneList.Clear();
            textblock_TargetData.Text = $"Target Data\nGeneration : {genetic_Main_Engine.generation.ToString()}";
            textblock_Data.Text = "Data";

            for (int i = 0; i < genetic_Main_Engine.genetic_target_data.Length; i++)
            {
                Canvas1.Children.Add(Create_Rectangle(i, Canvas1.Margin.Bottom - 140, genetic_Main_Engine.genetic_target_data[i], 1));
            }

            foreach (int[] gene in genetic_Main_Engine.gene_set)
            {
                int score = genetic_Main_Engine.Score_Calculation(genetic_Main_Engine.genetic_target_data, gene);

                for (int i = 0; i < gene.Length; i++)
                {
                    Rectangle rec = Create_Rectangle(i, index, gene[i], 0);
                    Canvas1.Children.Add(rec);

                }
                textbox_GeneList.AppendText($"index{index} Score: " + genetic_Main_Engine.Score_Calculation(gene, genetic_Main_Engine.genetic_target_data).ToString() + "\n");

                index++;
            }
            if (genetic_Main_Engine.Score_Calculation(genetic_Main_Engine.genetic_target_data, genetic_Main_Engine.gene_set[0]) == 0)
                MessageBox.Show("Calculation Completed!");
        }
    }
}




