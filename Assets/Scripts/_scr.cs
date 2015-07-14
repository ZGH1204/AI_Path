using UnityEngine;
using System.Collections;

public class _scr : MonoBehaviour {

    // copy_wt_=路墙 _begin=开始点 _end=结束点 msz_=数字 lsz_= 路
    public Transform copy_wt_, _begin, _end, msz_, lsz_;

    // 设定随机数
    System.Random sj_;

    // 产生随机种子
    int seekSeek = unchecked((int)System.DateTime.Now.Ticks);

    // 物体数组
    Transform[] xso_;

    // 产生物体数量
    int wtsl_ = 100; 

    // 20*20格 是否存在物体
    bool[,] wt_yn_ = new bool[21,21] ;
    int[,] wt_yn_int_ = new int[21, 21];

    // xy位置
    int sjx_, sjy_;

	void Start () {
        Debug.Log("QQ：23331122 小健 \n2014-04-06");
        Debug.Log("1、A键_刷新地形路线 \n2、鼠标左键_设置开始点 ");
        Debug.Log("3、鼠标右键_设置结束点 \n4、空格键_运算寻路");
        sj_ = new System.Random(seekSeek);
        xso_ = new Transform[wtsl_]; // 创建一个新物体数组
	}
	
	void Update () {
        // A*算法寻路
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // 开始点、结束点 必须在地图上
            Vector3 v3_begin = _begin.position ,v3_end = _end.position;
            if (v3_begin.x >= -10 && v3_begin.x <= 10 && v3_begin.z >= -10 && v3_begin.z <= 10 && v3_end.x >= -10 && v3_end.x <= 10 && v3_end.z >= -10 && v3_end.z <= 10)
            {
                // 调用公式，返回路程数组
                Vector2[] ok_v2_ = _Asuan_gs_(wt_yn_, new Vector2(v3_begin.x, v3_begin.z), new Vector2(v3_end.x, v3_end.z));
                int bcc_ = ok_v2_.Length;
                if (bcc_ > 0)
                {
                    Debug.Log("步长：" + bcc_);
                    if ((int)ok_v2_[0].x == (int)v3_end.x && (int)ok_v2_[0].y == (int)v3_end.z)
                    {
                        for (int vxy_ = 0; vxy_ < bcc_; vxy_++)
                        {
                            // Debug.Log("vxy_：" + vxy_ + " x：" + ok_v2_[vxy_].x + " y：" + ok_v2_[vxy_].y);

                            // msz_=数字 复制组件
                            Transform szs_ = Instantiate(lsz_, new Vector3(ok_v2_[vxy_].x, 0.2f, ok_v2_[vxy_].y), Quaternion.Euler(0, 0, 0)) as Transform;
                            // 将新组件物体层级修改到原来组件层
                            szs_.parent = msz_.parent;
                            // 将类 tag 改成 jjgam 方便以后群体删除
                            szs_.tag = "jjgam";
                        }
                        Debug.Log("A*算法完成 QQ：23331122 小健 2014-04-06");
                    }else
					{ 
						Debug.Log("无解_A_ QQ：23331122 小健 2014-04-06");
					}
                }
                else
                { 
					Debug.Log("无解_B_ QQ：23331122 小健 2014-04-06"); 
				}
            }else
            { 
				Debug.Log("无解 _ 开始点、结束点 必须在地图上 QQ：23331122 小健 2014-04-06");
			}
        }

        // 产生随机物体
        if (Input.GetKeyUp(KeyCode.A))
        {

            Initialize_we_(); // 初始化物体位置

            //地图大小 20*20 中心点 0,0  从 -10,-10 到 10,10
            for (int zc_ = 0; zc_ < wtsl_; zc_++)
            {
                bool gok_ = true;
                while (gok_)
                {
                    sjx_ = suiji(0, 20);
                    sjy_ = suiji(0, 20);
                    if (wt_yn_[sjx_, sjy_] == false)
                    {
                        wt_yn_[sjx_, sjy_] = true;
                        gok_ = false;
                        break; // 跳出
                    }
                }
                // 复制组件
                xso_[zc_] = Instantiate(copy_wt_, new Vector3(sjx_ - 10, 0.2f, sjy_ - 10), Quaternion.Euler(0, 0, 0)) as Transform;
                // 将新组件物体层级修改到原来组件层
                xso_[zc_].parent = copy_wt_.parent;
                // 获取附带在组件的脚本
                _ge ge_ = xso_[zc_].GetComponent<_ge>();
                // 设置数值
                ge_.xfg_ = suiji(0, 99);

            }
        }

        if (Input.GetMouseButtonUp(0)) // 左键按下
        {
            Vector2 xxyy_ = PlayerMove(); // 获取世界坐标
            // Debug.Log("xxyy_x_" + xxyy_.x + " xxyy_y_" + xxyy_.y);
            if ((int)xxyy_.x != -99 && wt_yn_[(int)xxyy_.x + 10, (int)xxyy_.y + 10] == false)
            {
                _begin.position = new Vector3(xxyy_.x, 0.2f, xxyy_.y);
            }
        }
        if (Input.GetMouseButtonUp(1)) // 右键按下
        {
            Vector2 xxyy_ = PlayerMove(); // 获取世界坐标
            // Debug.Log("xxyy_x_" + xxyy_.x + " xxyy_y_" + xxyy_.y);
            if ((int)xxyy_.x != -99 && wt_yn_[(int)xxyy_.x + 10, (int)xxyy_.y + 10] == false)
            {
                _end.position = new Vector3(xxyy_.x, 0.2f, xxyy_.y);
            }
        }
	
	}

    // A*逻辑 bss_已经存在物体的位置true，op_开始点，ed_结束点
    Vector2[] _Asuan_gs_(bool[,] bss_, Vector2 op_, Vector2 ed_)
    {
        bool QiuJie_ = false;
        Vector2[] v2_s_ = new Vector2[0] ;

        // 初始两个V2数组 用作交替存储扩散的位置
        Vector2[] lvup_ = new Vector2[0];
        Vector2[] lvup_B_ = new Vector2[0];

        copy_sz_(new Vector2(op_.x, op_.y), 0); // 开始点显示数字0
        // Debug.Log("op_.x_=" + op_.x + " op_.y_=" + op_.y);
        wt_yn_int_[(int)op_.x+10, (int)op_.y+10] = 0;
        wt_yn_[(int)op_.x + 10, (int)op_.y + 10] = true;//标记为存在物体

        lvup_ = add_v2_(lvup_, new Vector2(op_.x, op_.y)); // 添加临时V2数组，到下一个循环用作扩散

        int kscss_ = 1 ;
        bool kuosan_ = true ; // 扩散
        int wjx_, wjy_;
        int ed_x = (int)ed_.x + 10, ed_y = (int)ed_.y + 10;
        while (kuosan_)
        {
            lvup_B_ = lvup_; // 交替存储
            lvup_ = new Vector2[0]; // 清空数组
            for (int wps_ = 0; wps_ < lvup_B_.Length; wps_++)
            {
                // 中心点
                Vector2 zxd_ = new Vector2(lvup_B_[wps_].x, lvup_B_[wps_].y);
                // Debug.Log("zxd_x_=" + zxd_.x + " zxd_y_=" + zxd_.y);
                
                if (zxd_.x >= -10 && zxd_.x <= 10 && zxd_.y >= -10 && zxd_.y <= 10)
                {
					//向左扩散
                    Vector2 zxd_L = new Vector2(zxd_.x - 1, zxd_.y);
                    wjx_ = (int)zxd_L.x + 10;
                    wjy_ = (int)zxd_L.y + 10;
                    if (zxd_.x >= -10 && wjx_ >= 0 && wjx_ <= 20 && wjy_ >= 0 && wjy_ <= 20)
                    {
                        if (wt_yn_[wjx_, wjy_] == false)//如果此处没有物体或者没有被标记为数字，标记数字
                        {
                            wt_yn_[wjx_, wjy_] = true;
                            copy_sz_(zxd_L, kscss_); // 显示数字
                            wt_yn_int_[wjx_, wjy_] = kscss_;
                            lvup_ = add_v2_(lvup_, new Vector2(wjx_-10, wjy_-10)); // 添加临时V2数组，到下一个循环用作扩散
                        }
                    }
                    if (ed_x == wjx_ && ed_y == wjy_) // 成功，跳出
                    {
						QiuJie_ = true; 
						kuosan_ = false;
						break;
					};

					//向右扩散
                    Vector2 zxd_R = new Vector2(zxd_.x + 1, zxd_.y);
                    wjx_ = (int)zxd_R.x + 10;
                    wjy_ = (int)zxd_R.y + 10;
                    if (zxd_.x <= 10 && wjx_ >= 0 && wjx_ <= 20 && wjy_ >= 0 && wjy_ <= 20)
                    {
						if (wt_yn_[wjx_, wjy_] == false)//如果此处没有物体，标记数字
                        {
                            wt_yn_[wjx_, wjy_] = true;
                            copy_sz_(zxd_R, kscss_); // 显示数字
                            wt_yn_int_[wjx_, wjy_] = kscss_;
                            lvup_ = add_v2_(lvup_, new Vector2(wjx_ - 10, wjy_ - 10)); // 添加临时V2数组，到下一个循环用作扩散
                        }
                    }
                    if (ed_x == wjx_ && ed_y == wjy_) // 成功，跳出
                    { 
						QiuJie_ = true; 
						kuosan_ = false; 
						break; 
					};

					//向下扩散
                    Vector2 zxd_D = new Vector2(zxd_.x, zxd_.y - 1);
                    wjx_ = (int)zxd_D.x + 10;
                    wjy_ = (int)zxd_D.y + 10;
                    if (zxd_.y >= -10 && wjx_ >= 0 && wjx_ <= 20 && wjy_ >= 0 && wjy_ <= 20)
                    {
                        if (wt_yn_[wjx_, wjy_] == false)
                        {
                            wt_yn_[wjx_, wjy_] = true;
                            copy_sz_(zxd_D, kscss_); // 显示数字
                            wt_yn_int_[wjx_, wjy_] = kscss_;
                            lvup_ = add_v2_(lvup_, new Vector2(wjx_ - 10, wjy_ - 10)); // 添加临时V2数组，到下一个循环用作扩散
                        }
                    }
                    if (ed_x == wjx_ && ed_y == wjy_) // 成功，跳出
                    { 
						QiuJie_ = true; 
						kuosan_ = false;
						break; 
					};

					//向上扩散
                    Vector2 zxd_U = new Vector2(zxd_.x, zxd_.y + 1);
                    wjx_ = (int)zxd_U.x + 10;
                    wjy_ = (int)zxd_U.y + 10;
                    if (zxd_.y <= 10 && wjx_ >= 0 && wjx_ <= 20 && wjy_ >= 0 && wjy_ <= 20)
                    {
                        if (wt_yn_[wjx_, wjy_] == false)
                        {
                            wt_yn_[wjx_, wjy_] = true;
                            copy_sz_(zxd_U, kscss_); // 显示数字
                            wt_yn_int_[wjx_, wjy_] = kscss_;
                            lvup_ = add_v2_(lvup_, new Vector2(wjx_ - 10, wjy_ - 10)); // 添加临时V2数组，到下一个循环用作扩散
                        }
                    }
                    if (ed_x == wjx_ && ed_y == wjy_) // 成功，跳出
                    { 
						QiuJie_ = true; 
						kuosan_ = false;
						break;
					};

                }
            }
            
            kscss_++;
            if (kscss_ >= 200)
            { 
				kuosan_ = false;
				break; 
			};
        }

        // 步长
        int bc_ = kscss_ - 1;
        Debug.Log("_步长_bc_" + bc_);

        if (QiuJie_) // 成功求解，逆向
        {
            // 加入第一个点
            v2_s_ = add_v2_(v2_s_, new Vector2((int)ed_.x, (int)ed_.y));

            int ed_s_x = (int)ed_.x + 10, ed_s_y = (int)ed_.y + 10;
            for (int sv_ = 0; sv_ < bc_; sv_++)
            {
                // wwz_ = 倒数
                int wwz_ = bc_ - sv_;
                // Debug.Log("wwz_"+wwz_);
				//获取上下左右的坐标（）优先级：左-》右-》下-》上
                for (int es_ = 0; es_ < 4; es_++)
                {
                    int new_x_=-1, new_y_=-1 ;
                    if (es_ == 0)
                    {  new_x_ = ed_s_x - 1;  new_y_ = ed_s_y; }
                    else if (es_ == 1)
                    {  new_x_ = ed_s_x + 1;  new_y_ = ed_s_y; }
                    else if (es_ == 2)
                    {  new_x_ = ed_s_x ;  new_y_ = ed_s_y - 1; }
                    else if (es_ == 3)
                    {  new_x_ = ed_s_x ;  new_y_ = ed_s_y + 1; }
                    // Debug.Log(es_ + " wwz_" + wwz_ + " new_x_" + new_x_ + " new_y_" + new_y_);
                    if (new_x_ >= 0 && new_x_ <= 20 && new_y_ >= 0 && new_y_ <= 20)
                    {
                        if (wt_yn_int_[new_x_, new_y_] == wwz_)
                        {
                            v2_s_ = add_v2_(v2_s_, new Vector2(new_x_-10, new_y_-10));
                            // Debug.Log("wwz_" + wwz_ + " _ " + (new_x_ - 10) + "," + (new_y_ - 10));
                            ed_s_x = (int)new_x_;
                            ed_s_y = (int)new_y_;
                            break; // 跳出一重 
                        }
                    }
                }
                // break; // 跳出一重 
            }
        }
        return v2_s_;
    }

    Vector2[] add_v2_(Vector2[] adv2_s,Vector2 adv2_)
    {
        int gg_ = adv2_s.Length+1 ;
        Vector2[] av2_ = new Vector2[gg_];
        if (gg_ > 1)
        {
            for (int sqw_ = 0; sqw_ < gg_-1; sqw_++)
            {
                av2_[sqw_] = adv2_s[sqw_];
            }
        }
        av2_[gg_-1] = adv2_;
        return av2_;
    }

    // 产生数字 wz_ss_=产生位置 sz_=数字
    void copy_sz_(Vector2 wz_ss_, int sz_)
    {
        bool cs_ok_ = false;
        if (cs_ok_)
        {
            // msz_=数字 复制组件
            Transform szs_ = Instantiate(msz_, new Vector3(wz_ss_.x, 0.2f, wz_ss_.y), Quaternion.Euler(0, 0, 0)) as Transform;
            // 将新组件物体层级修改到原来组件层
            szs_.parent = msz_.parent;
            // 获取附带在组件的脚本
            _ge ge_ = szs_.GetComponent<_ge>();
            // 将类 tag 改成 jjgam 方便以后群体删除
            szs_.tag = "jjgam";
            // 设置数值
            ge_.xfg_ = sz_;
        }
        
    }

    // 鼠标点击返回坐标
    Vector2 PlayerMove()
    {
        Vector2 xy_s = new Vector2(-99,-99);
        //鼠标在屏幕上的位置
        Vector3 xy = Input.mousePosition;
        //在鼠标所在的屏幕位置发出一条射线(得出碰撞点)
        Ray ray = Camera.main.ScreenPointToRay(xy);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) //当射线彭转到对象时
        {
            // hit.collider.gameObject.tag
            // hit.collider.gameObject.name
            // Debug.Log(hit.collider.gameObject.name);
            if (hit.collider.gameObject.name == "DiMian_20x20")
            {
                // 输出鼠标点击世界坐标
                Vector3 xyz = hit.point;
                xy_s.x = (int)(hit.point.x);
                xy_s.y = (int)(hit.point.z);
                // Debug.Log("x：" + (int)(hit.point.x) + " y：" + (int)(hit.point.y) + " z：" + (int)(hit.point.z)); 
            }
        }
        return xy_s;
    }

    // 整数范围随机
    int suiji(int min_, int max_)
    {
        int xn_ = sj_.Next(min_, max_ + 1);
        return xn_;
    }

    // 初始化物体位置
    void Initialize_we_()
    {
        // 销毁原来物品
        for (int zc_ = 0; zc_ < wtsl_; zc_++)
        {
            if (xso_[zc_] != null)
            {
                Destroy(xso_[zc_].gameObject); // 销毁原来的物品
            }
        }

        for (int x_ = 0; x_ < 21; x_++)
        {
            for (int y_ = 0; y_ < 21; y_++)
            {
                wt_yn_[x_, y_] = false;
                wt_yn_int_[x_, y_] = -1;
            }
        }

        // 清空衍生出来的数字 _SZ(Clone)  // FindGameObjectsWithTag 获取tag数组
        GameObject[] jgame_ = GameObject.FindGameObjectsWithTag("jjgam");
        for (int jag_ = 0; jag_ < jgame_.Length; jag_++)
        { Destroy(jgame_[jag_]); }

        // 销毁所有路墙 重新定义
        xso_ = new Transform[wtsl_];

        // 复位两个物体 开始物体、结束物体
        _begin.position = new Vector3(-13,0.2f,5);
        _end.position = new Vector3(-13, 0.2f, 3);

    }

}
