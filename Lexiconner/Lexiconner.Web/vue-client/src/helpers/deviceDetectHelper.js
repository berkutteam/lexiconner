class DeviceDetectHelper {
  constructor() {
    throw new Error(`Can't crate an instance of the static class!`);
  }

  static checkIsMobile() {
    return window.matchMedia("only screen and (max-width: 760px)").matches;
  }
}

export default DeviceDetectHelper;
