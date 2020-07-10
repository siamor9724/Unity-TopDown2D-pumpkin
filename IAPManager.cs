using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public const string ProductGold = "Gold"; //comsumable
    public const string ProductSubscription = "premium_subscription"; //subscription

    private const string _iOS_GoldId = "com.studio.app.gold";//개발자센터에서 상품아이디를 사용
    private const string _android_GoldId = "com.nopg.gold_10000";

    private const string _iOS_PremiumSubscription = "com.studio.app.sub";//개발자센터에서 상품아이디를 사용, 해당앱 상품페이지에서 직접 작성해야한다
    private const string _android_PremiumSubscription = "com.studio.app.sub";

    private static IAPManager instance;

    public static IAPManager Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindObjectOfType<IAPManager>();
            if (instance == null) instance = new GameObject("IAP Manager").AddComponent<IAPManager>();
            return instance;
        }
    }

    private IStoreController storeController; //구매과정 제어
    private IExtensionProvider storeExtensionProvider;// 여러 플랫폼을 위한 확장 처리

    public bool isInitialized => storeController != null && storeExtensionProvider != null;
   /* public bool isInitialized
    {
        get
        {
            if(storeController != null && storeExtensionProvider != null)
            {
                return true;
            }
            return false;
        }
    }*/
    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        InitUnityIAP();
    }
    void InitUnityIAP() 
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct(
                ProductGold, ProductType.Consumable, new IDs() {
                    {_iOS_GoldId, AppleAppStore.Name },
                    {_android_GoldId, GooglePlay.Name }
                }
        );
        builder.AddProduct(
               ProductSubscription, ProductType.Subscription, new IDs() {
                    {_iOS_PremiumSubscription, AppleAppStore.Name },
                    {_android_PremiumSubscription, GooglePlay.Name }
               }
       );

        UnityPurchasing.Initialize(this, builder);
    } 

    public void OnInitialized(IStoreController contoller, IExtensionProvider extensions)
    {
        Debug.Log("초기화 성공");
        storeController = contoller;
        storeExtensionProvider = extensions;

    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"유니티 IAP초기화 실패{error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"구매상품 id: {args.purchasedProduct.definition.id}");
        if(args.purchasedProduct.definition.id == ProductGold)
        {
            Debug.Log("골드구매처리");
        }else if (args.purchasedProduct.definition.id == ProductSubscription)
        {
            Debug.Log("구독처리");
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogWarning($"구매실패 - {product.definition.id},{reason} ");
    }

    public void Purchase(string productId)
    {
        if (!isInitialized) return;

        var product = storeController.products.WithID(productId);
        if(product != null && product.availableToPurchase)
        {
            Debug.Log($"구매시도 - {product.definition.id}");
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.Log($"구매시도 불가 - {productId}");
        }
    }
    public void RestorePurchase()
    {
        if (!isInitialized) return;
        if(Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("구매복구시도");
            var appleExt = storeExtensionProvider.GetExtension<IAppleExtensions>();
            appleExt.RestoreTransactions(
                result => Debug.Log($"구매복구시도 결과 - {result}")
                );

        }
    }

    public bool HadPurchased(string productId)
    {
        if (!isInitialized) return false;

        var product = storeController.products.WithID(productId);
        if(product != null)
        {
            return product.hasReceipt;
        }

        return false;
    }
}
