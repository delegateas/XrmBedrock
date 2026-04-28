export class ContextMock<TInputs> {
    parameters: TInputs;
    updatedProperties: string[] = [];
    mode = {
        isControlDisabled: false,
        isVisible: true,
        label: "",
        setControlState: () => true,
        setFullScreen: () => { /* no-op */ },
        setNotification: () => true,
        clearNotification: () => true,
        trackContainerResize: () => { /* no-op */ }
    } as unknown as ComponentFramework.Mode;
    client = {
        getClient: () => "Web",
        getFormFactor: () => 2,
        isOffline: () => false,
        isNetworkAvailable: () => true,
        disableScroll: false
    } as ComponentFramework.Client;
    factory = {
        getPopupService: () => ({} as never),
        requestRender: () => { /* no-op */ }
    } as unknown as ComponentFramework.Factory;
    formatting = {} as ComponentFramework.Formatting;
    navigation = {} as ComponentFramework.Navigation;
    resources = {
        getString: (key: string) => key,
        getResource: () => { /* no-op */ }
    } as unknown as ComponentFramework.Resources;
    utils = {} as ComponentFramework.Utility;
    webAPI = {} as ComponentFramework.WebApi;
    userSettings = {
        userId: "00000000-0000-0000-0000-000000000000",
        userName: "Test User",
        languageId: 1033,
        isRTL: false,
        dateFormattingInfo: {} as never,
        numberFormattingInfo: {} as never,
        getTimeZoneOffsetMinutes: () => 0,
        securityRoles: []
    } as unknown as ComponentFramework.UserSettings;

    constructor(parameters: TInputs) { this.parameters = parameters; }
}
