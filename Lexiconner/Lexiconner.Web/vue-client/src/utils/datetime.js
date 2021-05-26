// import moment from 'moment';
import moment from "moment-timezone";
import store from "@/store";

/**
 * Formats - https://momentjscom.readthedocs.io/en/latest/moment/04-displaying/01-format/
 */
class DateTimeUtil {
  constructor() {}

  get userTimeZoneId() {
    if (
      store.state.userInfo &&
      store.state.userInfo.timeZone &&
      store.state.userInfo.timeZone.timeZoneId
    ) {
      return store.state.userInfo.timeZone.timeZoneId;
    } else {
      return null;
    }
  }

  formatToISO(datetime) {
    let format = "YYYY-MM-DDTHH:mm:ssZ";
    return moment(datetime).format(format);
  }

  // TimeZone
  _getInUserTimeZoneOrUTC(datetime) {
    if (this.userTimeZoneId) {
      console.log(this.userTimeZoneId);
      return moment(datetime).tz(this.userTimeZoneId);
    } else {
      // UTC
      return moment(datetime).utcOffset(0);
    }
  }

  getInDateInUTC(datetime) {
    return moment(datetime).utcOffset(0).format("YYYY-MM-DD");
  }

  getInTimeInUTC(datetime) {
    return moment(datetime).utcOffset(0).format("HH:mm");
  }

  formatInUserTimeZone(datetime) {
    return this._getInUserTimeZoneOrUTC(datetime).format();
  }

  // User
  formatToUserDisplayTime(datetime) {
    let format =
      ((store.userInfo || {}).formatSettings || {}).timeFormat || "HH:mm";
    return this._getInUserTimeZoneOrUTC(datetime).format(format);
  }

  formatToUserDisplayDate(datetime) {
    let format =
      ((store.userInfo || {}).formatSettings || {}).dateFormat || "YYYY-MM-DD";
    return this._getInUserTimeZoneOrUTC(datetime).format(format);
  }

  formatToUserDisplayDateTime(datetime) {
    let format =
      ((store.userInfo || {}).formatSettings || {}).dateTimeFormat ||
      "YYYY-MM-DD HH:mm";
    return this._getInUserTimeZoneOrUTC(datetime).format(format);
  }

  // Other
  formatToDefaultDisplayShortDateTimeInWeek(datetime, from = null, to = null) {
    let format = "ddd HH:mm"; // Mon 21:30
    return this._getInUserTimeZoneOrUTC(datetime).format(format);
  }

  formatToDefaultDisplayShortDateTimeInMonth(datetime, from = null, to = null) {
    let format = "MMM Do HH:mm"; // Dec 30th 21:30
    return this._getInUserTimeZoneOrUTC(datetime).format(format);
  }

  formatAccordingToInterval(datetime, from, to) {
    let duration = moment.duration(
      this._getInUserTimeZoneOrUTC(to).diff(this._getInUserTimeZoneOrUTC(from))
    );
    let intervalHours = duration.asHours();
    let intervalDays = duration.asDays();
    let intervalMonths = duration.asMonths();

    if (intervalHours <= 24) {
      // show only time
      return this.formatToUserDisplayTime(datetime);
    } else if (intervalDays <= 7) {
      // show short in week datetime
      return this.formatToDefaultDisplayShortDateTimeInWeek(datetime);
    } else if (intervalMonths <= 1) {
      // show short in month datetime
      return this.formatToDefaultDisplayShortDateTimeInMonth(datetime);
    } else {
      // show datetime
      return this.formatToUserDisplayDateTime(datetime);
    }
  }
}

export default new DateTimeUtil();
