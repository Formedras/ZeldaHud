﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace zeldaGui
{
	public partial class ZeldaHUD : Form
	{
		public ZeldaHUD()
		{
			InitializeComponent();

		}
		string currentIconSet = @"IconSets\Defaults";
		string currentBG = @"None";
		Bitmap backgroundimage;
		List<string> item_found = new List<string>();
		private void ZeldaHUD_Load(object sender, EventArgs e)
		{
			TimerImage.MakeTransparent(Color.Fuchsia);
			CountImage.MakeTransparent(Color.Fuchsia);
			Text = "Right Click Here";
			pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			g = Graphics.FromImage(pictureBox1.Image);

			AddItems();
			SetDefaultItems();

			loadLayout();
			LoadIconsSet(currentIconSet);
			if (currentBG != "None")
			{
				backgroundimage = new Bitmap(currentBG);
				backgroundimage.MakeTransparent(Color.Fuchsia);
			}
			DrawIcons();
			//if (checkUpdate)
			//{
			//    if (Version.CheckUpdate() == true)
			//    {
			//        var window = MessageBox.Show("There is a new version available do you want to download the update?", "Update Available", MessageBoxButtons.YesNo);
			//        if (window == DialogResult.Yes)
			//        {
			//            Help.ShowHelp(null, @"https://github.com/Formedras/ZeldaHUD/releases");
			//        }
			//    }
			//}

		}
		int nbrIcons = 74;
		public static Bitmap[] iconSet;
		public Bitmap TimerImage = new Bitmap("IconSets\\Default\\timer.png");
		public Bitmap CountImage = new Bitmap("IconSets\\Default\\count.png");

		public void LoadIconsSet(string data)
		{
			nbrIcons = Directory.GetFiles(data).Length;

			iconSet = new Bitmap[nbrIcons];
			for (int i = 0; i < nbrIcons; i++)
			{
				if (File.Exists(data + "\\" + i.ToString("D4") + ".png"))
				{
					iconSet[i] = new Bitmap(data + "\\" + i.ToString("D4") + ".png");
					iconSet[i].MakeTransparent(Color.Fuchsia);
				}
			}
			if (currentBG != "None")
			{
				backgroundimage = new Bitmap(currentBG);
				backgroundimage.MakeTransparent(Color.Fuchsia);
			}
			currentIconSet = data;
		}

		public void DrawIcons()
		{
			pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
			g = Graphics.FromImage(pictureBox1.Image);
			g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
			g.Clear(clearColor);

			ColorMatrix colorMatrix = new ColorMatrix(
			new float[][]
			{
				new float[] {.15f, .15f, .15f, 0, 0},
				new float[] {.16f, .16f, .16f, 0, 0},
				new float[] {.06f, .06f, .06f, 0, 0},
				new float[] {0, 0, 0,1f, 0},
				new float[] {0, 0, 0, 0f, 0f}
			});

			ImageAttributes ia = new ImageAttributes();
			ia.SetColorMatrix(colorMatrix);

			for (int x = 0; x < widthIcons; x++)
			{
				for (int y = 0; y < heightIcons; y++)
				{
					if (itemsArray[x, y] != null)
					{
						if ((currentBG != "None") && (backgroundimage != null) && (itemsArray[x, y].name != "Timer"))
						{
							g.DrawImage(backgroundimage, new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel);
						}
						if (itemsArray[x, y].name != "Timer")
						{
							if (itemsArray[x, y].on)
							{
								g.DrawImage(iconSet[itemsArray[x, y].iconsId[itemsArray[x, y].level]], new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel);
								if (itemsArray[x, y].count == true)
								{
									if (itemsArray[x, y].counter >= 1)
									{
										DrawCounter(g, x * 32, y * 32, itemsArray[x, y].counter);
									}
								}
							}
							else
							{
								g.DrawImage(iconSet[itemsArray[x, y].iconsId[itemsArray[x, y].level]], new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, ia);
							}
						}
						else if (!timer)
						{
							g.DrawImage(iconSet[itemsArray[x, y].iconsId[itemsArray[x, y].level]], new Rectangle(x * 32, y * 32, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, ia);
						}
					}
				}
			}
			if (timer)
			{
				TimeSpan objt = timerend ? timeended.Subtract(timestarted) : DateTime.Now.Subtract(timestarted);

				//g.DrawString(objt.Hours.ToString("D2") + ":" + objt.Minutes.ToString("D2") + ":" + objt.Seconds.ToString("D2"), label1.Font, Brushes.White, new Point(timerpospixel.X - 2, timerpospixel.Y + 4));
				DrawTime(g, objt);
			}

			pictureBox1.Refresh();
		}

		public void DrawCounter(Graphics g, int x, int y, byte count)
		{
			string s = count.ToString("D2");
			if (count <= 9)
			{
				g.DrawImage(CountImage, new Rectangle(x + 22, y + 18, 10, 14), count * 10, 0, 10, 14, GraphicsUnit.Pixel);
			}
			else
			{
				int b = (s[0] - 48);
				g.DrawImage(CountImage, new Rectangle(x + 12, y + 18, 10, 14), b * 10, 0, 10, 14, GraphicsUnit.Pixel);
				b = (s[1] - 48);
				g.DrawImage(CountImage, new Rectangle(x + 22, y + 18, 10, 14), b * 10, 0, 10, 14, GraphicsUnit.Pixel);
			}


			g.DrawImage(CountImage, new Rectangle((x * 32) + 22, (y * 32) + 18, 10, 14), 0, 0, 32, 32, GraphicsUnit.Pixel);
		}

		public void DrawTime(Graphics graphics, TimeSpan timeSpan)
		{

			string s = timeSpan.Hours.ToString("D2");
			int b = (s[0] - 48);
			graphics.DrawImage(TimerImage, timerpospixel.X + 1, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//hour1
			b = (s[1] - 48);
			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 13, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//hour2

			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 26, timerpospixel.Y + 4, new Rectangle(120, 0, 8, 26), GraphicsUnit.Pixel);//:
			s = timeSpan.Minutes.ToString("D2");
			b = (s[0] - 48);
			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 35, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//minute1
			b = (s[1] - 48);
			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 48, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//minute2

			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 61, timerpospixel.Y + 4, new Rectangle(120, 0, 8, 26), GraphicsUnit.Pixel);//:
			s = timeSpan.Seconds.ToString("D2");
			b = (s[0] - 48);
			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 69, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//second1
			b = (s[1] - 48);
			graphics.DrawImage(TimerImage, (timerpospixel.X + 1) + 82, timerpospixel.Y + 4, new Rectangle(12 * b, 0, 12, 26), GraphicsUnit.Pixel);//second2

		}


		protected void OnTitlebarClick(Point pos)
		{
			contextMenuStrip1.Show(pos);
		}


		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0xa4)
			{
				Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
				OnTitlebarClick(pos);
				return;
			}
			base.WndProc(ref m);
		}

		Graphics g;
		public static List<CustomItem> itemsList = new List<CustomItem>();
		public static CustomItem[,] itemsArray = new CustomItem[24, 24];
		public static Color clearColor = Color.FromArgb(10, 10, 15);

		public void AddItems()
		{
			string[] s = File.ReadAllLines("itemlist.txt");

			foreach (string line in s)
			{
				string name = "";
				byte[] order = new byte[0];
				bool loop = false;
				bool bottle = false;
				bool count = false;
				//Read character
				if (line.Length > 1)
				{
					if (line[0] == '/' && line[1] == '/')//skip line
					{

					}
					else
					{
						if (line[0] == ':')
						{
							for (int i = 1; i < line.Length; i++)//name
							{
								if (line[i] == ',')
								{
									break;
								}
								name += line[i];
							}
							int sta = line.IndexOf('{') + 1;
							int end = line.IndexOf('}');
							string ord = line.Substring(sta, end - sta);
							string[] ords = ord.Split(',');
							order = new byte[ords.Length];
							for (int i = 0; i < ords.Length; i++)
							{
								order[i] = Convert.ToByte(ords[i]);
							}
							string last = line.Substring(end + 2);
							string[] loopbottle = last.Split(',');
							if (loopbottle[0] == "false")
							{
								loop = false;
							}
							if (loopbottle[0] == "true")
							{
								loop = true;
							}

							if (loopbottle[1] == "false")
							{
								bottle = false;
							}
							if (loopbottle[1] == "true")
							{
								bottle = true;
							}

							if (loopbottle[2][0] == 'f')
							{
								count = false;
							}
							if (loopbottle[2][0] == 't')
							{
								count = true;
							}
							CustomItem ci = new CustomItem(order, name, loop, bottle, count);
							if (count == true)
							{
								ci.on = true;
							}
							itemsList.Add(ci);

						}
					}
				}
			}
		}


		public void SetDefaultItems()
		{
			//30,31,32,17,25,60,36,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
			byte[] ditems = new byte[] {15 ,0 ,1 ,2 ,3 ,41,30,
										16,5 ,6 ,7 ,8 ,9 ,31,
										18,10,11,42,13, 14,32,
										27,46,44,255,255,45,17,
										19,22,61,62,51,24,25,
										20,21,40,12,4,23,60,
										34,37,39,38,35,33,36};
			int i = 0;
			for (int y = 0; y < 7; y++)
			{
				for (int x = 0; x < 7; x++)
				{

					if (itemsList.Count >= ditems[i])
					{
						itemsArray[x, y] = itemsList[ditems[i]];

					}
					i++;
				}
			}
		}


		byte widthIcons = 7;
		byte heightIcons = 6;
		private void ZeldaHUD_ResizeEnd(object sender, EventArgs e)
		{

			//Resize the form to always be a multiple of 32
			int w = (Size.Width / 32) * 32;
			int h = (Size.Height / 32) * 32;
			if (w >= 24 * 32)
			{
				w = (24 * 32);
			}
			if (h >= 24 * 32)
			{
				h = (24 * 32);
			}
			if (h <= 64)
			{
				h = 64;
			}
			if (w <= 48)
			{
				w = 32;
			}
			Size = new Size((w) + 16, (h) + 7);
			widthIcons = (byte)(w / 32);
			heightIcons = (byte)((h / 32) - 1);
			DrawIcons();

		}
		bool editMode = false;
		private void ToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			if (toolStripMenuItem1.Checked)
			{
				editMode = true;
				FormBorderStyle = FormBorderStyle.Sizable;
			}
			else
			{
				editMode = false;
				FormBorderStyle = FormBorderStyle.FixedSingle;
			}
		}
		bool mDown = false;
		bool startDrag = true;
		CustomItem dragged = null;
		CustomItem swapped = null;
		int xdpos = -1;
		int ydpos = -1;
		private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
		{
			if (editMode == true)
			{
				if (mDown == false)
				{
					uiChanged = true;
					dragged = itemsArray[(e.X / 32), (e.Y / 32)];
					startDrag = true;
					xdpos = (e.X / 32);
					ydpos = (e.Y / 32);
					Cursor = Cursors.Hand;
					mDown = true;
				}
			}
		}

		private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
		{
			if (editMode == true)
			{
				if (mDown == true)
				{
					if (startDrag == true)
					{
						if ((e.X / 32) < 24 && (e.X / 32) >= 0)
						{
							if ((e.Y / 32) < 24 && (e.Y / 32) >= 0)
							{
								uiChanged = true;
								swapped = itemsArray[(e.X / 32), (e.Y / 32)];
								itemsArray[(e.X / 32), (e.Y / 32)] = dragged;
								if (dragged != null)
								{
									if (dragged.name == "Timer")
									{
										timerpospixel = new Point((e.X / 32) * 32, (e.Y / 32) * 32);
									}
								}
								itemsArray[xdpos, ydpos] = swapped;
								swapped = null;
								dragged = null;
								startDrag = false;
								xdpos = -1;
								ydpos = -1;
								Cursor = Cursors.Default;
								mDown = false;
							}
						}
					}
				}
			}
			DrawIcons();
		}


		DateTime timestarted;
		DateTime timeended;
		bool timerend = false;
		bool uiChanged = false;
		private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			int mX = (e.X / 32);
			int mY = (e.Y / 32);
			if (editMode == false)
			{
				if (itemsArray[mX, mY] != null)
				{
					if (e.Button == MouseButtons.Left)
					{


						if (itemsArray[mX, mY].on)
						{
							if (itemsArray[mX, mY].count)
							{
								itemsArray[mX, mY].counter++;
							}

							if (itemsArray[mX, mY].level < itemsArray[mX, mY].iconsId.Length - 1)
							{
								if (itemsArray[mX, mY].name == "Timer_StartDone")
								{
									timeended = DateTime.Now;
									timerend = true;
								}
								//if (itemsArray)
								if (itemsArray[mX, mY].bottle)
								{
									if (itemsArray[mX, mY].level == 0)
									{
										itemsArray[mX, mY].level += 2;
									}
									else
									{
										if (itemsArray[mX, mY].level < 6)
										{
											itemsArray[mX, mY].level++;
										}
									}
								}
								else
								{
									itemsArray[mX, mY].level++;
								}
								//drawIcons();
							}
							else
							{
								if (itemsArray[mX, mY].level == itemsArray[mX, mY].iconsId.Length - 1)
								{
									if (itemsArray[mX, mY].loop == true)
									{
										itemsArray[mX, mY].level = 0;
									}
								}
							}

						}
						else
						{
							if (itemsArray[mX, mY].name == "Timer_Start" || itemsArray[mX, mY].name == "Timer_StartDone")
							{

								for (int x = 0; x < widthIcons; x++)
								{
									for (int y = 0; y < heightIcons; y++)
									{
										if (itemsArray[x, y] != null)
										{
											if (itemsArray[x, y].name == "Timer")
											{
												timerpospixel = new Point(x * 32, y * 32);
												timer2.Enabled = true;
												timer = true;
												timestarted = DateTime.Now;
												break;
											}
										}
									}
								}
							}
							if (itemsArray[mX, mY].name == "Timer_Done")
							{
								timeended = DateTime.Now;
								timerend = true;
							}
							itemsArray[mX, mY].on = true;
							//drawIcons();
						}
						TimeSpan objt = DateTime.Now.Subtract(timestarted);
						if (itemsArray[mX, mY].loop == false)
						{
							item_found.Add(itemsArray[mX, mY].name + " Added at " + objt.Hours.ToString("D2") + ":" + objt.Minutes.ToString("D2") + ":" + objt.Seconds.ToString("D2"));
						}
					}
					else if (e.Button == MouseButtons.Right)
					{
						if (itemsArray[mX, mY].on)
						{
							if (itemsArray[mX, mY].name == "Timer_Start" || itemsArray[mX, mY].name == "Timer_StartDone")
							{
								if (itemsArray[mX, mY].level == 0)
								{
									timer = false;
								}
							}


							if (itemsArray[mX, mY].count == false)
							{
								if (itemsArray[mX, mY].name == "Timer_Done" || itemsArray[mX, mY].name == "Timer_StartDone")
								{

									timerend = false;
								}
								TimeSpan objt = DateTime.Now.Subtract(timestarted);
								if (itemsArray[mX, mY].loop == false)
								{
									item_found.Add(itemsArray[mX, mY].name + " Removed at " + objt.Hours.ToString("D2") + ":" + objt.Minutes.ToString("D2") + ":" + objt.Seconds.ToString("D2"));
								}
								if (itemsArray[mX, mY].level > 0)
								{
									itemsArray[mX, mY].level--;
									if (itemsArray[mX, mY].bottle)
									{
										if (itemsArray[mX, mY].level == 1)
										{
											itemsArray[mX, mY].level--;
										}
									}
								}
								else if (itemsArray[mX, mY].level == 0)
								{
									if (itemsArray[mX, mY].loop == false)
									{
										itemsArray[mX, mY].on = false;
									}
								}
							}
							else
							{
								itemsArray[mX, mY].counter--;
							}
						}
					}
				}
			}
			else
			{
				if (e.Button == MouseButtons.Right)
				{
					ItemSelectorForm itemForm = new ItemSelectorForm();
					if (itemForm.ShowDialog() == DialogResult.OK)
					{
						if (itemForm.selectedItem == 255)
						{
							itemsArray[mX, mY] = null;
						}
						else
						{
							itemsArray[mX, mY] = itemsList[itemForm.selectedItem];
						}
					}
				}
			}
			DrawIcons();
		}

		private void ToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			OptionsForm of = new OptionsForm();
			of.checkBox1.Checked = checkUpdate;
			of.iconset = currentIconSet;
			of.bgr = currentBG;
			of.ShowDialog();
			checkUpdate = of.checkBox1.Checked;
			currentBG = of.bgr;
			LoadIconsSet(of.iconset);
			DrawIcons();
		}

		private void TopMostToolStripMenuItem_Click(object sender, EventArgs e)
		{
			TopMost = !TopMost;
			topMostToolStripMenuItem.Checked = TopMost;
		}
		bool autoUpdate = false;
		private void toolStripMenuItem3_Click(object sender, EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
			{
				toolStripMenuItem3.Checked = false;
				timer1.Enabled = false;
			}
			else
			{
				toolStripMenuItem3.Checked = true;
				autoUpdate = true;
				timer1.Enabled = true;
			}

		}

		private void ZeldaHUD_FormClosing(object sender, CancelEventArgs e)
		{
			if (uiChanged == true)
			{
				if (MessageBox.Show("Your Layout has changed do you want to save?", "Warning",
				   MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					//e.Cancel = true;
					save();
				}
			}
		}






		public void AutoUpdateHud()
		{
			if (autoUpdate)
			{
				if (File.Exists(openFileDialog1.FileName))
				{
					try
					{
						byte[] buffer = new byte[255];
						FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read)
						{
							Position = 0x1E00
						};
						fs.Read(buffer, 0, 255);
						fs.Close();

						//0 - 14 bow to book of mudora
						//15 = bottle not used in my list
						//(16 - 23 somaria to moon pearl) -1 list
						for (int i = 0; i < 15; i++)
						{
							if (buffer[i] == 0)
							{
								itemsList[i].level = 0;
								itemsList[i].on = false;
							}
							else
							{
								if (i != 1)//bomerang
								{
									if (buffer[i] == 1)
									{
										itemsList[i].on = true;
									}
								}
								else if (i != 4) // Mushroom
								{
									if (buffer[i] == 1)
									{
										itemsList[i].on = true;
									}
								}
								else if (i != 12) // Shovel
								{
									if (buffer[i] == 1)
									{
										itemsList[i].on = true;
									}
								}
								else
								{
									itemsList[i].on = true;
								}
								if (i != 3) // bombs
								{
									if (i != 1)//bomerang
									{
										if (i != 4) // Mushroom
										{
											if (i != 12) // Shovel
											{
												itemsList[i].level = (byte)(buffer[i] - 1);
												itemsList[i].on = true;
											}
											else
											{
												if (buffer[i] >= 2)//if it the shovel and buffer >= 2
												{
													itemsList[42].on = true; //flute = on
												}
											}
										}
										else
										{
											if (buffer[i] == 2)//if it the mushroom and buffer == 2
											{
												itemsList[41].on = true; //powder = on
											}
										}
									}
									else
									{
										if (buffer[i] == 2)//if it the boomerang and buffer == 2
										{
											itemsList[40].on = true; //red boomerang = on
										}
									}
								}
								else
								{
									itemsList[i].on = true;
								}


							}
							else
							{
								itemsList[i].on = false;
								itemsList[i].level = 0;
								//itemsList[i].level = (byte)(buffer[i]-1);
							}
						}
						itemsList[51].on = true;
						if (buffer[43] == 0)
						{
							itemsList[51].level = 0;
						}
						if (buffer[43] == 1)
						{
							itemsList[51].level = 1;
						}
						if (buffer[43] == 2)
						{
							itemsList[51].level = 2;
						}
						if (buffer[43] == 3)
						{
							itemsList[51].level = 3;
						}

						for (int i = 16; i < 24; i++)
						{
							if (buffer[i] != 0)
							{
								itemsList[i - 1].on = true;
								itemsList[i - 1].level = (byte)(buffer[i] - 1);
							}
						}

						for (int i = 24; i < 32; i++)
						{
							if (i == 27)
							{
								itemsList[i - 2].on = true;
							}
							if (buffer[i] != 0)
							{
								itemsList[i - 2].on = true;
								if (i != 27)
								{
									itemsList[i - 2].level = (byte)(buffer[i] - 1);
								}
								else
								{

									itemsList[i - 2].level = buffer[i];
								}
							}
						}
						int adddoubles = 210;
						if (buffer[adddoubles] != 96)
						{
							if ((buffer[adddoubles] & 1) == 1)//flute working 42
							{
								itemsList[42].on = true;
							}

							if ((buffer[adddoubles] & 2) == 2)//flute fake 42
							{
								itemsList[42].on = true;
							}
							if ((buffer[adddoubles] & 4) == 4)//shovel 12
							{
								itemsList[12].on = true;
							}
							if ((buffer[adddoubles] & 16) == 16)//powder 41
							{
								itemsList[41].on = true;
							}
							if ((buffer[adddoubles] & 32) == 32)//mush 4
							{
								itemsList[4].on = true;
							}
							if ((buffer[adddoubles] & 64) == 64)//red boom 40
							{
								itemsList[40].on = true;
							}
							if ((buffer[adddoubles] & 128) == 128)//blue boom 1
							{
								itemsList[1].on = true;
							}
						}
						BitConverter.ToInt16(new byte[2] { 0, 1 }, 0);
						if ((buffer[52] & 1) == 1) //hera
						{
							itemsList[32].on = true;
						}
						if ((buffer[52] & 2) == 2) //desert
						{
							itemsList[31].on = true;
						}
						if ((buffer[52] & 4) == 4) //eastern
						{
							itemsList[30].on = true;
						}
						if ((buffer[133]) == 3) //agahnim
						{
							itemsList[43].on = true;
						}


						if ((buffer[58] & 1) == 1)//mire
						{
							itemsList[33].on = true;
						}
						if ((buffer[58] & 2) == 2)//pod
						{
							itemsList[34].on = true;
						}
						if ((buffer[58] & 4) == 4)//ice
						{
							itemsList[35].on = true;
						}
						if ((buffer[58] & 8) == 8)//trock
						{
							itemsList[36].on = true;
						}
						if ((buffer[58] & 16) == 16)//swamp
						{
							itemsList[37].on = true;
						}
						if ((buffer[58] & 32) == 32)//tt
						{
							itemsList[38].on = true;
						}
						if ((buffer[58] & 64) == 64)//sw
						{
							itemsList[39].on = true;
						}


						DrawIcons();
					}
					catch (Exception e)
					{
						Text = e.Message.ToString();
					}
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			//Autoupdate
			AutoUpdateHud();
		}

		private void helpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://zarby89.github.io/ZeldaHud/");
		}
		public bool checkUpdate = true;
		private void save()
		{
			string[] config = new string[10];
			config[0] = "width=" + widthIcons;
			config[1] = "height=" + heightIcons;
			config[2] = "items=" + stringitems();
			byte b = 0;
			if (TopMost)
			{ b = 1; }
			else
			{ b = 0; }
			config[3] = "topmost=" + b;
			config[4] = "winposx=" + Location.X;
			config[5] = "winposy=" + Location.Y;
			config[6] = "color=" + clearColor.ToArgb();
			config[7] = "iconset=" + currentIconSet;
			if (checkUpdate)
			{ b = 1; }
			else
			{ b = 0; }
			config[8] = "checkupdate=" + b;
			config[9] = "background=" + currentBG;

			File.WriteAllLines("layout.config", config);
		}
		private void saveLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			save();
			uiChanged = false;
		}

		public void loadLayout(string sx = "Layout.config")
		{
			if (File.Exists(sx))
			{
				string[] s = File.ReadAllLines(sx);
				string[] itl = s[2].Split('=')[1].Split(',');
				int w = Convert.ToInt32(s[0].Split('=')[1]);
				int h = Convert.ToInt32(s[1].Split('=')[1]);
				Location = new Point(Convert.ToInt32(s[4].Split('=')[1]), Convert.ToInt32(s[5].Split('=')[1]));
				int b = Convert.ToInt32(s[3].Split('=')[1]);
				if (b == 1)
				{ TopMost = true; topMostToolStripMenuItem.Checked = true; }
				else
				{ TopMost = false; topMostToolStripMenuItem.Checked = false; }
				b = Convert.ToInt32(s[8].Split('=')[1]);
				if (b == 1)
				{ checkUpdate = true; }
				else
				{ checkUpdate = false; }
				clearColor = Color.FromArgb(Convert.ToInt32(s[6].Split('=')[1]));
				currentIconSet = s[7].Split('=')[1];
				if (s.Length >= 10)
				{
					currentBG = s[9].Split('=')[1];
				}



				int p = 0;
				for (int x = 0; x < 24; x++)
				{
					for (int y = 0; y < 24; y++)
					{
						if (Convert.ToInt32(itl[p]) != -1)
						{
							itemsArray[x, y] = itemsList[Convert.ToInt32(itl[p])];
						}
						else
						{
							itemsArray[x, y] = null;
						}
						p++;
					}
				}
				Size = new Size((w * 32) + 16, ((h + 1) * 32) + 7);
				widthIcons = (byte)(w);
				heightIcons = (byte)((h));
				DrawIcons();
			}
		}


		private string stringitems()
		{
			string s = "";
			int p = 0;
			for (int x = 0; x < 24; x++)
			{
				for (int y = 0; y < 24; y++)
				{
					if (itemsArray[x, y] != null)
					{
						s += itemsList.FindIndex(i => i.Equals(itemsArray[x, y])).ToString();
						s += ",";
					}
					else
					{
						s += "-1,";
					}
					p++;
				}
			}
			return s;
		}

		private void clearItemsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int x = 0; x < 24; x++)
			{
				for (int y = 0; y < 24; y++)
				{
					if (itemsArray[x, y] != null)
					{
						itemsArray[x, y].on = false;
						itemsArray[x, y].level = 0;
					}
				}
			}
			DrawIcons();
		}

		private void showStatsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			timestarted = DateTime.Now;
		}

		private void showStatsToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Stats f = new Stats();
			for (int i = 0; i < item_found.Count; i++)
			{
				f.richTextBox1.AppendText(item_found[i] + "\n");
			}
			f.ShowDialog();
		}

		private void imageStatsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Bitmap b = new Bitmap(288, 384);
			Bitmap bgr = new Bitmap("bgrtimeico.png");
			Graphics g = Graphics.FromImage(b);
			int y = 0;
			int x = 0;
			for (int i = 0; i < item_found.Count; i++)
			{
				string s = item_found[i].Substring(item_found[i].Length - 8, 8);
				string[] ss = item_found[i].Split(' ');
				g.DrawImage(bgr, new Point(x * 72, y * 24));
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
				g.DrawImage(iconSet[Convert.ToInt32(ss[0])], new Rectangle((x * 72) + 2, (y * 24) + 4, 16, 16), 0, 0, 32, 32, GraphicsUnit.Pixel);
				g.DrawString(s, new Font(label1.Font, FontStyle.Regular), Brushes.White, new PointF((x * 72) + 20, (y * 24) + 05));
				x++;
				if (x >= 4)
				{
					y++;
					x = 0;
				}
			}
			b.Save("Test.png");

		}
		bool timer = false;
		Point timerpospixel;
		private void timer2_Tick(object sender, EventArgs e)
		{
			DrawIcons();
		}
	}
}
