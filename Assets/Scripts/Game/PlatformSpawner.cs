using UnityEngine;

/// <summary>
/// 平台生成器
/// </summary>
/// 

public enum PlatformGroupType
{
    Grass,
    Winter,
    Common,
}

public class PlatformSpawner : MonoBehaviour
{
    private ManagerVars vars;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }

    public Vector3 startSpawnPos;   //平台生成初始位置(-0.9, -4, 0)
    private int spawnPlatformCount;  //生成平台数量个数
    private Vector3 platformSpawnPosition; // 平台生成位置
    private Sprite selectPlatform;  // 随机到的平台主题
    private bool isLeftSpawn = false;  // 判断是向左还是向右生成平台
    private PlatformGroupType groupType;   // 组合平台的类型
    private bool spikeSpawnLeft = false;  // 钉子组合平台是否生成在左边，反之右边
    private Vector3 spikeDirPlatformPos;   // 钉子平台方向位置
    private int afterSpawnSpikeCount;   // 生成钉子平台之后需要在钉子方向上生成平台的数量
    private bool isSpawnSpike;  // 是否生成钉子
    public int mileStoneCount = 10;   // 定义里程碑
    public float fallTiem; // 掉落时间
    public float minFallTime;  // 最小掉落时间
    public float multiple;  // 


    private void OnEnable()
    {
        RandomPlatformTheme();
        platformSpawnPosition = startSpawnPos;
        for (int i = 0; i < 5; i++) // 一开始生成5个
        {
            spawnPlatformCount = 5;
            DecidePath();
        }

        GameObject go = Instantiate(vars.characterPre); // 生成人物
        go.transform.localPosition = vars.defaultPos;
      
    }
    private void Update()
    {
        if (GameManager.Instance.IsGameStarted && GameManager.Instance.IsGameOver == false)
        {
            UpdateFallTime();
        }
    }

    /// <summary>
    /// 更新平台掉落时间
    /// </summary>
    private void UpdateFallTime()
    {
        if (GameManager.Instance.GetGameScore > mileStoneCount)
        {
            mileStoneCount *= 2;
            fallTiem *= multiple;
            if (fallTiem < minFallTime)
            {
                fallTiem = minFallTime;
            }
        }
    }

    /// <summary>
    /// 生成路径
    /// </summary>
    private void DecidePath()
    {
        if (isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }
        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
            SpawnPlatform();
        }
        else
        {
            isLeftSpawn = !isLeftSpawn;  // 转换平台生成方向
            spawnPlatformCount = Random.Range(1, 4);  // 随机一次转向一次
            SpawnPlatform();
        }
    }

    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        int ranObstacleDir = Random.Range(0, 2);   // 随机生成障碍平台左右

        if (spawnPlatformCount >= 1)  // 生成单个平台
        {
            SpawnNormalPlatform(ranObstacleDir);
        }

        else if (spawnPlatformCount == 0)  // 生成组合平台
        {
            int ran = Random.Range(0, 3);
            // 生成通用组合平台
            if (ran == 0)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);
            }
            // 生成主题 组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Common:
                        SpawnCommonPlatformGroup(ranObstacleDir);
                        break;

                    default:
                        break;
                }
            }
            // 生成钉子组合平台
            else
            {
                int value = -1;   // 标志钉子左右，0为右边，1为左边
                if (isLeftSpawn)
                {
                    value = 0;
                }
                else
                {
                    value = 1;  // 生成左边方向的钉子
                }
                SpawnSpikePlatform(value);
                isSpawnSpike = true;
                afterSpawnSpikeCount = 5; // 钉子后面的跳台数量
                if (spikeSpawnLeft)  // 钉子在左边
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPosition.x - 1.65f, platformSpawnPosition.y + vars.nextYPos, 0);
                }
                else
                {
                    spikeDirPlatformPos = new Vector3(platformSpawnPosition.x + 1.65f, platformSpawnPosition.y + vars.nextYPos, 0);
                }
            }
        }

        // 生成钻石
        int ranSpawnDiamond = Random.Range(0, 8);
        if (ranSpawnDiamond >= 5 && GameManager.Instance.PlayerIsMove)
        {
            GameObject go = ObjectPool.Instance.GetDiamond();
            go.transform.position = new Vector3(platformSpawnPosition.x,
                platformSpawnPosition.y + 0.5f, 0);
            go.SetActive(true);
        }

        if (isLeftSpawn)  // 向左生成
        {
            platformSpawnPosition = new Vector3(platformSpawnPosition.x - vars.nextXPos,
                platformSpawnPosition.y + vars.nextYPos, 0);
        }
        else // 向右生成
        {
            platformSpawnPosition = new Vector3(platformSpawnPosition.x + vars.nextXPos,
               platformSpawnPosition.y + vars.nextYPos, 0);
        }

    }

    /// <summary>
    /// 生成单个平台
    /// </summary>
    private void SpawnNormalPlatform(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();  // 生成并设为该对象的子物体
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatform, fallTiem, ranObstacleDir);
        go.SetActive(true);
    }

    /// <summary>
    /// 随机平台主题
    /// </summary>
    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platfromSpriteList.Count);
        selectPlatform = vars.platfromSpriteList[ran];

        if (ran == 2) // 冬季主题
        {
            groupType = PlatformGroupType.Winter;
        }
        else if (ran == 1)  // 草地组合
        {
            groupType = PlatformGroupType.Grass;
        }
        else
        {
            groupType = PlatformGroupType.Common;
        }
    }

    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetCommonPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatform, fallTiem, ranObstacleDir);  //传入选中的组合平台
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetGrassPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatform, fallTiem, ranObstacleDir);  //传入选中的组合平台   
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetWinterPlatform();
        go.transform.position = platformSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatform, fallTiem, ranObstacleDir);  //传入选中的组合平台
        go.SetActive(true);
    }

    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    private void SpawnSpikePlatform(int dir)
    {
        GameObject temp = null;
        if (dir == 0)
        {
            spikeSpawnLeft = false;
            temp = ObjectPool.Instance.GetSpikePlatformRight();
        }
        else
        {
            spikeSpawnLeft = true;
            temp = ObjectPool.Instance.GetSpikePlatformLeft();
        }
        temp.transform.position = platformSpawnPosition;
        temp.GetComponent<PlatformScript>().Init(selectPlatform, fallTiem, dir);
        temp.SetActive(true);
    }

    /// <summary>
    /// 生成钉子平台之后需要生成的平台
    /// 包括钉子方向，也包括原来的方向
    /// </summary>
    private void AfterSpawnSpike()
    {
        if (afterSpawnSpikeCount > 0)
        {
            afterSpawnSpikeCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject temp = ObjectPool.Instance.GetNormalPlatform();
                if (i == 0) // 生成原来路径的平台
                {
                    temp.transform.position = platformSpawnPosition;
                    // 如果钉子在左边，原先路径在右边
                    if (spikeSpawnLeft)
                    {
                        platformSpawnPosition = new Vector3(platformSpawnPosition.x + vars.nextXPos,
                            platformSpawnPosition.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        platformSpawnPosition = new Vector3(platformSpawnPosition.x - vars.nextXPos,
                            platformSpawnPosition.y + vars.nextYPos, 0);
                    }
                }
                else  // 生成钉子方向的平台
                {
                    temp.transform.position = spikeDirPlatformPos;
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXPos,
                            spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXPos,
                            spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                }
                temp.GetComponent<PlatformScript>().Init(selectPlatform, fallTiem, 1);
                temp.SetActive(true);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
    }
}
