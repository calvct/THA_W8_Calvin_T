using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;

namespace THA_W8_Calvin_T
{
    public partial class Form1 : Form
    {
        Label[] label1;
        Label[] answer = new Label[11];
        MySqlConnection conn = new MySqlConnection();
        MySqlDataAdapter sqlDataAdapter = new MySqlDataAdapter();
        MySqlCommand sqlCommand = new MySqlCommand();
        string host = "server=localhost;uid=root;pwd=;database=premier_league;";
        string query = "";
        DataTable dtteam = new DataTable();
        DataTable dtselect = new DataTable();
        DataTable dtpemain = new DataTable();
        DataTable dtmatch = new DataTable();
        DataTable dtselected = new DataTable();
        DataTable dthome = new DataTable();
        DataTable dtaway = new DataTable();
        List<string> datalist = new List<string>() { "Player Name:","Team Name:", "Nationality:", "Position:", "Squad Number:","Yellow Card:","Red Card:","Goal Scored:","Own Goal:","Goal Penalty:","Penalty Miss:"};
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(host);
        }

        private void playerDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtteam.Clear();
            dtselect.Clear();
            dtpemain.Clear();
            dtselected.Clear();
            panel2.Controls.Clear();
            panel1.Controls.Clear();
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            cmbox_select.Visible = true;
            cmbox_selected.Visible = true;
            panel1.Visible = true;
            panel2.Visible = true;
            query = "select team_id as 'ID', team_name as 'Team Name' from team;";
            sqlCommand = new MySqlCommand(query, conn);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dtteam);
            cmbox_select.DataSource = dtteam;
            cmbox_select.ValueMember = "ID";
            cmbox_select.DisplayMember = "Team Name";
            cmbox_select.Text = "";
        }


        private void cmbox_select_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if(panel1.Visible == true)
            {
                dtselect.Clear();
                dtpemain.Clear();
                dtselected.Clear();
                panel1.Controls.Clear();
                panel2.Controls.Clear();
                cmbox_selected.Text = "";
                query = $"select p.player_name as 'Player Name', p.player_id as 'ID' from player p, team t where p.team_id = t.team_id and t.team_id = '{cmbox_select.SelectedValue.ToString()}';";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dtselect);
                cmbox_selected.DataSource = dtselect;
                cmbox_selected.ValueMember = "ID";
                cmbox_selected.DisplayMember = "Player Name";
            }
            else
            {
                dtselect.Clear();
                cmbox_selected.Text = "";
                query = $"select d.match_id as 'Match ID', date_format(m.match_date, '%d %M %Y') as 'Match Date' from dmatch d, `match` m where d.match_id = m.match_id and d.team_id = '{cmbox_select.SelectedValue.ToString()}' group by 1 order by m.match_date;";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dtselect);
                cmbox_selected.DataSource = dtselect;
                cmbox_selected.ValueMember = "Match ID";
                cmbox_selected.DisplayMember = "Match Date";
            }
            
        }

        private void cmbox_selected_SelectionChangeCommitted_1(object sender, EventArgs e)
        {
            if(panel1.Visible == true)
            {
                dtpemain.Clear();
                dtselected.Clear();
                panel2.Controls.Clear() ;
                label1 = new Label[datalist.Count];
                for (int i = 0; i < label1.Length; i++)
                {
                    label1[i] = new Label();
                    label1[i].Location = new Point(0, (i * 20) + 0);
                    label1[i].AutoSize = true;
                    label1[i].Text = datalist[i];
                    label1[i].TextAlign = ContentAlignment.MiddleCenter;
                    panel1.Controls.Add(label1[i]);
                }
                query = $"select p.player_name as 'Player Name', t.team_name as 'Team Name', n.nation as 'Nationality', if (p.playing_pos = 'F','FW',if (p.playing_pos = 'M','MF',if (p.playing_pos = 'D','DF','GK'))) as 'Position', p.team_number as 'Squad Number' from player p, team t, nationality n, dmatch d where t.team_id = p.team_id and n.nationality_id = p.nationality_id and p.player_id = '{cmbox_selected.SelectedValue.ToString()}' and p.team_id = t.team_id group by 1;";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dtpemain);
                query = $"select ifnull(sum(if(d.`type` = 'CY',1,0)),0) as 'Yellow Card', ifnull(sum(if(d.`type` = 'CR',1,0)),0) as 'Red Card', ifnull(sum(if(d.`type` = 'GO',1,0)),0) as 'Goal', ifnull(sum(if(d.`type` = 'GW',1,0)),0) as 'Own Goal', ifnull(sum(if(d.`type` = 'GP',1,0)),0) as 'Goal Penalty', ifnull(sum(if(d.`type` = 'PM',1,0)),0) as 'Penalty Miss' from dmatch d, player p where p.player_id = d.player_id and p.player_id = '{cmbox_selected.SelectedValue.ToString()}';";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dtselected);
                for (int i = 0; i < 5; i++)
                {
                    answer[i] = new Label();
                    answer[i].Location = new Point(0, (i * 20) + 0);
                    answer[i].AutoSize = true;

                    answer[i].Text = dtpemain.Rows[0][i].ToString();

                    panel2.Controls.Add(answer[i]);
                }
                for(int i = 5; i < answer.Length; i++)
                {
                    answer[i] = new Label();
                    answer[i].Location = new Point(0, (i * 20) + 0);
                    answer[i].AutoSize = true;

                    answer[i].Text = dtselected.Rows[0][i-5].ToString();

                    panel2.Controls.Add(answer[i]);
                }
            }
            else
            {
                dthome.Clear();
                dtaway.Clear();
                dtmatch.Clear();
                query = $"select p.player_name as 'Home Team Player', if(p.playing_pos='F','FW',if(p.playing_pos='M','MF',if(p.playing_pos='D','DF','GK'))) as 'Position',  t.team_name as 'Team Name' from team t, player p, `match` m where t.team_id = m.team_home and p.team_id = t.team_id and m.match_id = '{cmbox_selected.SelectedValue.ToString()}';";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dthome);

                query = $"select p.player_name as 'Away Team Player', if(p.playing_pos='F','FW',if(p.playing_pos='M','MF',if(p.playing_pos='D','DF','GK'))) as 'Position', t.team_name as 'Team Name' from team t, player p, `match` m where t.team_id = m.team_away and p.team_id = t.team_id and m.match_id = '{cmbox_selected.SelectedValue.ToString()}';";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dtaway);

                query = $"select d.`minute` as 'Minute', t.team_name as 'Team name', p.player_name as 'Player Name', if(d.`type` = 'CY', 'Yellow Card',if(d.`type` = 'CR', 'Red Card',if(d.`type` = 'GO', 'Goal', if(d.`type` = 'GW','Own Goal',if(d.`type` = 'GP', 'Goal Penalty','Penalty Miss'))))) as 'Type' from dmatch d, player p, team t where d.team_id = t.team_id and p.player_id = d.player_id and d.match_id = '{cmbox_selected.SelectedValue.ToString()}';";
                sqlCommand = new MySqlCommand(query, conn);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(dtmatch);
                dataGridView1.DataSource = dthome;
                dataGridView2.DataSource = dtaway;
                dataGridView3.DataSource = dtmatch;

            }
            
        }

        private void showMatchDetailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dtteam.Clear();
            dtselect.Clear();
            dthome.Clear();
            dtaway.Clear();
            dtmatch.Clear();
            panel1.Visible = false;
            panel2.Visible = false;
            cmbox_select.Visible = true;
            cmbox_selected.Visible = true;
            dataGridView1.Visible = true;
            dataGridView2.Visible = true;
            dataGridView3.Visible = true;
            query = "select team_id as 'ID', team_name as 'Team Name' from team;";
            sqlCommand = new MySqlCommand(query, conn);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(dtteam);
            cmbox_select.DataSource = dtteam;
            cmbox_select.ValueMember = "ID";
            cmbox_select.DisplayMember = "Team Name";
            cmbox_select.Text = "";
            cmbox_selected.Text = "";
        }
    }
}
